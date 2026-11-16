import base64
import datetime
import functools
import json
import os
import re
import unicodedata
import uuid
from http.server import SimpleHTTPRequestHandler, ThreadingHTTPServer

PORT = int(os.environ.get("PORT", "8081"))
ALAP = os.path.abspath(os.path.join(os.path.dirname(os.path.abspath(__file__)), ".."))
JSON_UTVONAL = os.path.join(ALAP, "mintaadatok.json")
KEPEK = os.path.join(ALAP, "kepek")
MAX_KEP = 5 * 1024 * 1024
MAX_KERES = 200 * 1024 * 1024
NEV_MINTA = re.compile(r"^[a-z0-9_]+\.(jpg|png)$")
DATUM_MINTA = re.compile(r"^\d{4}-\d{2}-\d{2}$")
AZONOSITO_MINTA = re.compile(r"^bj-(\d+)$")
KATEGORIAK = {"Egyeb", "Uthiba", "Kozvilagitas", "Hulladek", "Utcabutor", "Forgalomtechnika", "Parkok", "ElhagyottJarmu", "Kozmu"}
ALLAPOTOK = ["Uj", "Ellenorizve", "Folyamatban", "LezarasraVar", "Lezart", "Teves", "HataskoronKivuli", "Osszevont"]
SZERVEZETEK = ["Utfenntartas", "Koztisztasag", "Kozvilagitas"]
FO_ALLAPOTOK = {"Ellenorizve", "Folyamatban", "LezarasraVar"}
KOTELEZO_ELLENORZES = {"Ellenorizve", "Folyamatban", "LezarasraVar", "Lezart"}
NINCS_RESZFELADAT = {"Uj", "Ellenorizve", "Teves", "HataskoronKivuli", "Osszevont"}


def alap_reszfeladat(x):
    return {
        "szervezet": x.get("szervezet"),
        "cim": x.get("cim", ""),
        "hatarido": x.get("hatarido"),
        "kesz": bool(x.get("kesz", False)),
        "megjegyzes": x.get("megjegyzes"),
        "elotte": x.get("elotte"),
        "utana": x.get("utana"),
    }


def alap_rekord(r, azonosito):
    kiegeszito = {k: v for k, v in r.items() if k not in KULCSOK}
    rekord = {
        "azonosito": r.get("azonosito") or azonosito,
        "kovetokod": r.get("kovetokod") or str(uuid.uuid4()),
        "cim": r.get("cim", ""),
        "leiras": r.get("leiras", ""),
        "helyszin": r.get("helyszin", ""),
        "szelesseg": r.get("szelesseg"),
        "hosszusag": r.get("hosszusag"),
        "kategoria": r.get("kategoria", "Egyeb"),
        "bekuldes": r.get("bekuldes"),
        "ellenorzes": r.get("ellenorzes"),
        "allapot": r.get("allapot", "Uj"),
        "veszelyes": bool(r.get("veszelyes", False)),
        "hatarido": r.get("hatarido"),
        "lezaras": r.get("lezaras"),
        "ertekeles": r.get("ertekeles"),
        "foAzonosito": r.get("foAzonosito"),
        "indok": r.get("indok"),
        "kepek": list(r.get("kepek", [])),
        "reszfeladatok": [alap_reszfeladat(x) for x in r.get("reszfeladatok", [])],
    }
    rekord.update(kiegeszito)
    return rekord


KULCSOK = {
    "azonosito", "kovetokod", "cim", "leiras", "helyszin", "szelesseg", "hosszusag", "kategoria",
    "bekuldes", "ellenorzes", "allapot", "veszelyes", "hatarido", "lezaras", "ertekeles",
    "foAzonosito", "indok", "kepek", "reszfeladatok",
}


def kovetkezo_azonosito(lista):
    szamok = []
    for r in lista:
        egyezes = AZONOSITO_MINTA.match(str(r.get("azonosito", "")))
        if egyezes:
            szamok.append(int(egyezes.group(1)))
    return "bj-%04d" % (max(szamok, default=0) + 1)


def normalizal(nyers):
    kimenet = []
    for r in nyers:
        kimenet.append(alap_rekord(r, kovetkezo_azonosito(kimenet + [x for x in nyers if x.get("azonosito")])))
    return kimenet


def lista_ir(lista):
    ideiglenes = JSON_UTVONAL + ".tmp"
    with open(ideiglenes, "w", encoding="utf-8", newline="\n") as f:
        json.dump(lista, f, ensure_ascii=False, indent=2)
        f.write("\n")
    os.replace(ideiglenes, JSON_UTVONAL)


def lista_olvas():
    if not os.path.exists(JSON_UTVONAL):
        return []
    with open(JSON_UTVONAL, encoding="utf-8-sig") as f:
        szoveg = f.read().strip()
    nyers = json.loads(szoveg) if szoveg else []
    lista = normalizal(nyers)
    if lista != nyers:
        lista_ir(lista)
    return lista


def kepnevek(r):
    nevek = list(r.get("kepek", []))
    for x in r.get("reszfeladatok", []):
        for kulcs in ("elotte", "utana"):
            if x.get(kulcs):
                nevek.append(x[kulcs])
    return nevek


def letezo_kepek():
    if not os.path.isdir(KEPEK):
        return []
    return sorted(n for n in os.listdir(KEPEK) if NEV_MINTA.match(n))


def allapot():
    return {"lista": lista_olvas(), "letezoKepek": letezo_kepek()}


def ekezetnelkuli(szoveg):
    bontott = unicodedata.normalize("NFD", szoveg)
    tiszta = "".join(c for c in bontott if not unicodedata.combining(c)).lower()
    tiszta = re.sub(r"[^a-z0-9]+", "_", tiszta).strip("_")
    return tiszta[:40] or "bejelentes"


def kep_kiterjesztes(adat):
    if adat[:8] == b"\x89PNG\r\n\x1a\n":
        return ".png"
    if adat[:3] == b"\xff\xd8\xff" and adat[-2:] == b"\xff\xd9":
        return ".jpg"
    return None


def szam(ertek, also, felso, nev):
    if isinstance(ertek, bool) or not isinstance(ertek, (int, float)):
        raise ValueError("A " + nev + " kötelező szám.")
    if not also <= ertek <= felso:
        raise ValueError("A " + nev + " " + str(also) + " és " + str(felso) + " közé essen.")
    return ertek


def datum(ertek, nev):
    if ertek in (None, ""):
        return None
    szoveg = str(ertek)
    if not DATUM_MINTA.match(szoveg):
        raise ValueError("A(z) " + nev + " ÉÉÉÉ-HH-NN formátumú legyen.")
    try:
        datetime.date.fromisoformat(szoveg)
    except ValueError:
        raise ValueError("A(z) " + nev + " érvénytelen dátum.")
    return szoveg


def reszfeladatok_ellenoriz(nyers):
    kimenet = []
    for x in nyers or []:
        szervezet = x.get("szervezet")
        if szervezet not in SZERVEZETEK:
            raise ValueError("Ismeretlen szervezet a részfeladatnál.")
        cim = str(x.get("cim", "")).strip()
        if not 3 <= len(cim) <= 160:
            raise ValueError("A részfeladat címe 3 és 160 karakter közötti legyen.")
        hatarido = datum(x.get("hatarido"), "részfeladat határideje")
        if not hatarido:
            raise ValueError("A részfeladat határideje kötelező.")
        kesz = bool(x.get("kesz"))
        megjegyzes = str(x.get("megjegyzes") or "").strip()
        if len(megjegyzes) > 1000:
            raise ValueError("A részfeladat megjegyzése legfeljebb 1000 karakter lehet.")
        if kesz and not megjegyzes:
            raise ValueError("Készre jelentett részfeladatnál a megjegyzés kötelező.")
        kimenet.append({
            "szervezet": szervezet,
            "cim": cim,
            "hatarido": hatarido,
            "kesz": kesz,
            "megjegyzes": megjegyzes or None,
            "elotte": None,
            "utana": None,
        })
    return kimenet


def rekord_ellenoriz(adat):
    if not isinstance(adat, dict):
        raise ValueError("Hibás adat.")
    cim = str(adat.get("cim", "")).strip()
    leiras = str(adat.get("leiras", "")).strip()
    helyszin = str(adat.get("helyszin", "")).strip()
    if not 5 <= len(cim) <= 120:
        raise ValueError("A cím 5 és 120 karakter közötti legyen.")
    if not 20 <= len(leiras) <= 2000:
        raise ValueError("A leírás 20 és 2000 karakter közötti legyen.")
    if not 5 <= len(helyszin) <= 200:
        raise ValueError("A helyszín 5 és 200 karakter közötti legyen.")
    kategoria = adat.get("kategoria")
    if kategoria not in KATEGORIAK:
        raise ValueError("Ismeretlen kategória.")
    ertekeles = adat.get("ertekeles")
    if ertekeles in ("", None):
        ertekeles = None
    elif isinstance(ertekeles, bool) or not isinstance(ertekeles, int) or not 1 <= ertekeles <= 5:
        raise ValueError("Az értékelés 1 és 5 közötti egész szám legyen.")
    indok = str(adat.get("indok") or "").strip() or None
    fo_azonosito = str(adat.get("foAzonosito") or "").strip() or None
    rekord = {
        "cim": cim,
        "leiras": leiras,
        "helyszin": helyszin,
        "szelesseg": szam(adat.get("szelesseg"), -90, 90, "szélesség"),
        "hosszusag": szam(adat.get("hosszusag"), -180, 180, "hosszúság"),
        "kategoria": kategoria,
        "bekuldes": datum(adat.get("bekuldes"), "beküldés ideje"),
        "ellenorzes": datum(adat.get("ellenorzes"), "ellenőrzés dátuma"),
        "allapot": adat.get("allapot") or "Uj",
        "veszelyes": bool(adat.get("veszelyes")),
        "hatarido": datum(adat.get("hatarido"), "határidő"),
        "lezaras": datum(adat.get("lezaras"), "lezárás dátuma"),
        "ertekeles": ertekeles,
        "foAzonosito": fo_azonosito,
        "indok": indok,
        "kepek": [],
        "reszfeladatok": reszfeladatok_ellenoriz(adat.get("reszfeladatok")),
    }
    sors_ellenoriz(rekord)
    return rekord


def sors_ellenoriz(r):
    allapot_neve = r["allapot"]
    if allapot_neve not in ALLAPOTOK:
        raise ValueError("Ismeretlen állapot.")
    rfk = r["reszfeladatok"]
    kesz_db = sum(1 for x in rfk if x["kesz"])
    bekuldes, ellenorzes, hatarido, lezaras = r["bekuldes"], r["ellenorzes"], r["hatarido"], r["lezaras"]

    if allapot_neve == "Uj" and (ellenorzes or hatarido):
        raise ValueError("Új állapotnál nincs ellenőrzés dátuma és határidő.")
    if allapot_neve in KOTELEZO_ELLENORZES and not (ellenorzes and hatarido):
        raise ValueError("Ennél az állapotnál az ellenőrzés dátuma és a határidő kötelező.")
    if allapot_neve in NINCS_RESZFELADAT and rfk:
        raise ValueError("Ennél az állapotnál nem lehet részfeladat.")
    if allapot_neve == "Folyamatban" and (not rfk or kesz_db == len(rfk)):
        raise ValueError("Folyamatban állapotnál legyen legalább egy részfeladat, és legalább egy még ne legyen kész.")
    if allapot_neve in ("LezarasraVar", "Lezart") and (not rfk or kesz_db != len(rfk)):
        raise ValueError("Ennél az állapotnál legyen részfeladat, és mind legyen kész.")
    if allapot_neve == "Lezart" and not lezaras:
        raise ValueError("Lezárt ügynél a lezárás dátuma kötelező.")
    if allapot_neve != "Lezart" and (lezaras or r["ertekeles"] is not None):
        raise ValueError("Lezárás dátuma és értékelés csak lezárt ügynél adható meg.")
    if allapot_neve in ("Teves", "HataskoronKivuli"):
        if not r["indok"] or len(r["indok"]) > 80:
            raise ValueError("Téves és hatáskörön kívüli ügynél az indok kötelező, legfeljebb 80 karakter.")
    elif r["indok"]:
        raise ValueError("Indok csak téves vagy hatáskörön kívüli ügynél adható meg.")
    if allapot_neve == "Osszevont":
        if not r["foAzonosito"]:
            raise ValueError("Összevont ügynél a főbejelentés kötelező.")
    elif r["foAzonosito"]:
        raise ValueError("Főbejelentés csak összevont ügynél adható meg.")

    if (ellenorzes or lezaras) and not bekuldes:
        raise ValueError("Add meg a beküldés idejét.")
    if ellenorzes and ellenorzes < bekuldes:
        raise ValueError("Az ellenőrzés nem lehet a beküldés előtt.")
    if hatarido and ellenorzes and hatarido < ellenorzes:
        raise ValueError("A határidő nem lehet az ellenőrzés előtt.")
    if lezaras and ellenorzes and lezaras < ellenorzes:
        raise ValueError("A lezárás nem lehet az ellenőrzés előtt.")
    for x in rfk:
        if hatarido and x["hatarido"] > hatarido:
            raise ValueError("A részfeladat határideje nem lehet későbbi az ügy határidejénél.")


def kereszt_ellenoriz(lista, r):
    if r["allapot"] == "Osszevont":
        fo = next((x for x in lista if x["azonosito"] == r["foAzonosito"]), None)
        if fo is None or fo is r:
            raise ValueError("A főbejelentés nem található.")
        if fo["kategoria"] != r["kategoria"]:
            raise ValueError("A főbejelentés kategóriája egyezzen az összevont ügyével.")
        if fo["allapot"] not in FO_ALLAPOTOK:
            raise ValueError("A főbejelentés legyen ellenőrzött és még nyitott (Ellenőrizve, Folyamatban vagy Lezárásra vár).")
    hivatkozok = [x for x in lista if x.get("foAzonosito") == r["azonosito"] and x is not r]
    if hivatkozok:
        if r["allapot"] not in FO_ALLAPOTOK:
            raise ValueError("Összevont bejelentések hivatkoznak rá, ezért nyitott, ellenőrzött állapotú maradjon.")
        if any(x["kategoria"] != r["kategoria"] for x in hivatkozok):
            raise ValueError("Összevont bejelentések hivatkoznak rá, ezért a kategóriája nem változhat.")


def hivatkozott(lista, nev):
    return any(nev in kepnevek(r) for r in lista)


def kep_torol(nev):
    if not NEV_MINTA.match(nev):
        return
    utvonal = os.path.join(KEPEK, nev)
    if os.path.isfile(utvonal):
        os.remove(utvonal)


def mentes(adat):
    lista = lista_olvas()
    bejovo = adat.get("rekord")
    rekord = rekord_ellenoriz(bejovo)
    index = adat.get("index")
    regi_nevek = set()
    regi_kepek = []
    if index is not None:
        if not isinstance(index, int) or not 0 <= index < len(lista):
            raise ValueError("Ismeretlen bejelentés.")
        regi = lista[index]
        regi_nevek = set(kepnevek(regi))
        regi_kepek = list(regi.get("kepek", []))
        rekord["azonosito"] = regi["azonosito"]
        rekord["kovetokod"] = regi["kovetokod"]
    else:
        rekord["azonosito"] = kovetkezo_azonosito(lista)
        rekord["kovetokod"] = str(uuid.uuid4())

    foglalt = {n for r in lista for n in kepnevek(r)}
    alap = ekezetnelkuli(rekord["cim"])
    irando = []

    def uj_kep(kep, utotag):
        try:
            bajtok = base64.b64decode(kep.get("adat", ""), validate=True)
        except ValueError:
            raise ValueError("A kép adata hibás.")
        if len(bajtok) > MAX_KEP:
            raise ValueError("A kép legfeljebb 5 MB lehet.")
        kiterjesztes = kep_kiterjesztes(bajtok)
        if kiterjesztes is None:
            raise ValueError("Csak JPEG vagy PNG kép tölthető fel.")
        sorszam = 1
        nev = alap + utotag + kiterjesztes
        while nev in foglalt or os.path.exists(os.path.join(KEPEK, nev)):
            sorszam += 1
            nev = alap + utotag + "_" + str(sorszam) + kiterjesztes
        foglalt.add(nev)
        irando.append((nev, bajtok))
        return nev

    rekord["kepek"] = [n for n in adat.get("megtartott", []) if n in regi_kepek]
    for kep in adat.get("ujKepek", []):
        rekord["kepek"].append(uj_kep(kep, ""))

    bejovo_rfk = bejovo.get("reszfeladatok") or []
    for i, rf in enumerate(rekord["reszfeladatok"]):
        forras = bejovo_rfk[i]
        for kulcs, utotag in (("elotte", "_r%d_elotte" % (i + 1)), ("utana", "_r%d_utana" % (i + 1))):
            uj = forras.get(kulcs + "Uj")
            megtart = forras.get(kulcs + "Megtart")
            if uj:
                rf[kulcs] = uj_kep(uj, utotag)
            elif megtart and megtart in regi_nevek:
                rf[kulcs] = megtart

    rekord = alap_rekord(rekord, rekord["azonosito"])
    if index is None:
        lista.append(rekord)
    else:
        lista[index] = rekord
    kereszt_ellenoriz(lista, rekord)

    os.makedirs(KEPEK, exist_ok=True)
    for nev, bajtok in irando:
        with open(os.path.join(KEPEK, nev), "wb") as f:
            f.write(bajtok)
    lista_ir(lista)

    for nev in regi_nevek:
        if not hivatkozott(lista, nev):
            kep_torol(nev)
    return allapot()


def torles(adat):
    lista = lista_olvas()
    index = adat.get("index")
    if not isinstance(index, int) or not 0 <= index < len(lista):
        raise ValueError("Ismeretlen bejelentés.")
    torolni = lista[index]
    if any(x.get("foAzonosito") == torolni["azonosito"] for x in lista if x is not torolni):
        raise ValueError("Összevont bejelentések hivatkoznak erre, ezért nem törölhető. Előbb azokat módosítsd vagy töröld.")
    lista.pop(index)
    lista_ir(lista)
    for nev in kepnevek(torolni):
        if not hivatkozott(lista, nev):
            kep_torol(nev)
    return allapot()


class Kezelo(SimpleHTTPRequestHandler):
    def end_headers(self):
        self.send_header("Cache-Control", "no-store")
        super().end_headers()

    def valasz(self, kod, objektum):
        torzs = json.dumps(objektum, ensure_ascii=False).encode("utf-8")
        self.send_response(kod)
        self.send_header("Content-Type", "application/json; charset=utf-8")
        self.send_header("Content-Length", str(len(torzs)))
        self.end_headers()
        self.wfile.write(torzs)

    def do_GET(self):
        if self.path.split("?")[0] == "/api/lista":
            try:
                self.valasz(200, allapot())
            except (OSError, ValueError) as hiba:
                self.valasz(500, {"hiba": "A mintaadatok.json nem olvasható: " + str(hiba)})
            return
        super().do_GET()

    def do_POST(self):
        utvonal = self.path.split("?")[0]
        muveletek = {"/api/mentes": mentes, "/api/torles": torles}
        if utvonal not in muveletek:
            self.valasz(404, {"hiba": "Ismeretlen végpont."})
            return
        try:
            hossz = int(self.headers.get("Content-Length", "0"))
            if hossz <= 0 or hossz > MAX_KERES:
                raise ValueError("Hibás kérésméret.")
            adat = json.loads(self.rfile.read(hossz).decode("utf-8"))
            if not isinstance(adat, dict):
                raise ValueError("Hibás kérés.")
            self.valasz(200, muveletek[utvonal](adat))
        except ValueError as hiba:
            self.valasz(400, {"hiba": str(hiba)})
        except OSError as hiba:
            self.valasz(500, {"hiba": "Fájlhiba: " + str(hiba)})


if __name__ == "__main__":
    kezelo = functools.partial(Kezelo, directory=ALAP)
    with ThreadingHTTPServer(("127.0.0.1", PORT), kezelo) as szerver:
        print("A generátor itt érhető el: http://localhost:" + str(PORT) + "/Generator/index.html")
        print("Leállítás: Ctrl+C vagy az ablak bezárása.")
        szerver.serve_forever()
