import base64
import functools
import json
import os
import re
import unicodedata
from http.server import SimpleHTTPRequestHandler, ThreadingHTTPServer

PORT = int(os.environ.get("PORT", "8081"))
ALAP = os.path.abspath(os.path.join(os.path.dirname(os.path.abspath(__file__)), ".."))
JSON_UTVONAL = os.path.join(ALAP, "mintaadatok.json")
KEPEK = os.path.join(ALAP, "kepek")
MAX_KEP = 5 * 1024 * 1024
MAX_KERES = 200 * 1024 * 1024
NEV_MINTA = re.compile(r"^[a-z0-9_]+\.(jpg|png)$")
DATUM_MINTA = re.compile(r"^\d{4}-\d{2}-\d{2}$")
KATEGORIAK = {"Egyeb", "Uthiba", "Kozvilagitas", "Hulladek", "Utcabutor"}


def lista_olvas():
    if not os.path.exists(JSON_UTVONAL):
        return []
    with open(JSON_UTVONAL, encoding="utf-8-sig") as f:
        szoveg = f.read().strip()
    return json.loads(szoveg) if szoveg else []


def lista_ir(lista):
    ideiglenes = JSON_UTVONAL + ".tmp"
    with open(ideiglenes, "w", encoding="utf-8", newline="\n") as f:
        json.dump(lista, f, ensure_ascii=False, indent=2)
        f.write("\n")
    os.replace(ideiglenes, JSON_UTVONAL)


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
    bekuldes = adat.get("bekuldes")
    if bekuldes is not None and not DATUM_MINTA.match(str(bekuldes)):
        raise ValueError("A beküldés dátuma ÉÉÉÉ-HH-NN formátumú legyen.")
    return {
        "cim": cim,
        "leiras": leiras,
        "helyszin": helyszin,
        "szelesseg": szam(adat.get("szelesseg"), -90, 90, "szélesség"),
        "hosszusag": szam(adat.get("hosszusag"), -180, 180, "hosszúság"),
        "kategoria": kategoria,
        "bekuldes": bekuldes,
        "kepek": [],
    }


def hivatkozott(lista, nev):
    return any(nev in r.get("kepek", []) for r in lista)


def kep_torol(nev):
    if not NEV_MINTA.match(nev):
        return
    utvonal = os.path.join(KEPEK, nev)
    if os.path.isfile(utvonal):
        os.remove(utvonal)


def mentes(adat):
    lista = lista_olvas()
    rekord = rekord_ellenoriz(adat.get("rekord"))
    index = adat.get("index")
    regi = []
    megtartott = []
    if index is not None:
        if not isinstance(index, int) or not 0 <= index < len(lista):
            raise ValueError("Ismeretlen bejelentés.")
        regi = list(lista[index].get("kepek", []))
        megtartott = [n for n in adat.get("megtartott", []) if n in regi]

    foglalt = {n for r in lista for n in r.get("kepek", [])}
    alap = ekezetnelkuli(rekord["cim"])
    uj_nevek = []
    irando = []
    for kep in adat.get("ujKepek", []):
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
        nev = alap + kiterjesztes
        while nev in foglalt or os.path.exists(os.path.join(KEPEK, nev)):
            sorszam += 1
            nev = alap + "_" + str(sorszam) + kiterjesztes
        foglalt.add(nev)
        uj_nevek.append(nev)
        irando.append((nev, bajtok))

    os.makedirs(KEPEK, exist_ok=True)
    for nev, bajtok in irando:
        with open(os.path.join(KEPEK, nev), "wb") as f:
            f.write(bajtok)

    rekord["kepek"] = megtartott + uj_nevek
    if index is None:
        lista.append(rekord)
    else:
        lista[index] = rekord
    lista_ir(lista)

    for nev in regi:
        if nev not in megtartott and not hivatkozott(lista, nev):
            kep_torol(nev)
    return allapot()


def torles(adat):
    lista = lista_olvas()
    index = adat.get("index")
    if not isinstance(index, int) or not 0 <= index < len(lista):
        raise ValueError("Ismeretlen bejelentés.")
    torolt = lista.pop(index)
    lista_ir(lista)
    for nev in torolt.get("kepek", []):
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
