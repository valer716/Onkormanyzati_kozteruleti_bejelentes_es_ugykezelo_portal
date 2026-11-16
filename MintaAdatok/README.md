# Mintaadatok

Mintabejelentések a bemutatóhoz és a kipróbáláshoz.

| Elem | Mire való |
|---|---|
| `mintaadatok.json` | A mintabejelentések listája |
| `kepek/` | A bejelentésekhez tartozó fényképek |
| `Generator/` | Böngészős űrlap, amivel új mintabejelentés vehető fel |

## A mintaadatok eredete

A 538 bejelentés a járokelo.hu nyilvános, győri bejelentéseiből készült 2021-től, tanulási célra. Átvettük a címet, a leírást, a helyszínt, a koordinátát, a kategóriát, az állapotot, a beküldés napját és a bejelentés első fotóját. A bejelentő neve, a hivatali válaszok és a hozzászólások nem kerültek át.

Az ellenőrzés napja az első hivatali válasz napja, a lezárás napja a megoldottnak jelölés napja, ahol az oldalon volt ilyen adat. Kitalált, a portál szabályainak megfelelő adat: ahol ilyen nem volt, ott ez a két nap, továbbá a határidő, az értékelés, a részfeladatok, a naplóbejegyzések időpontja és az, hogy melyik demó-felhasználó végezte őket. A járókelős állapotok leképezése: megoldott → Lezart, megoldatlan → Teves (külső szolgáltatónál HataskoronKivuli), megoldásra vár → Folyamatban, a többi → Uj.

## Betöltés a portálba

Fejlesztői módban a portál az első indításkor betölti a `mintaadatok.json` fájlt és a `kepek/` mappát, ha a bejelentések tábla üres. Részletek a gyökér `README.md` fájlban.

## A generátor használata

1. Indítsd el a `Generator/Inditas.bat` fájlt (Python kell hozzá). Elindul egy kis helyi szerver, és megnyílik a `http://localhost:8081/Generator/index.html` oldal.
2. Az **Új bejelentés** fülön töltsd ki az űrlapot úgy, mintha a lakos küldené be: cím, leírás, helyszín, hely a térképen, kategória, fényképek.
3. A **3. Az ügy sorsa** részben add meg, hol tart az ügy: állapot, határidő, részfeladatok (szervezet, határidő, kész-e, megjegyzés, előtte/utána kép), lezárás és értékelés, összevonás, indok. A nem releváns mezők az állapot választása után elrejtőznek, és a szerver a portál szabályai szerint ellenőriz (például lezárt ügynél minden részfeladat kész).
4. A **Mentés** a `mintaadatok.json` fájlhoz fűz egy elemet, a képeket a `kepek/` mappába teszi.
5. A **Szerkesztő** fülön látszanak a meglévő bejelentések, itt szerkeszthetők és törölhetők.

A szerver csak a saját gépről érhető el, és a fájlokat közvetlenül a `MintaAdatok` mappába írja. Bármelyik böngészővel használható.

## Egy bejelentés a fájlban

```json
{
  "azonosito": "bj-0001",
  "kovetokod": "329572",
  "cim": "Betört aknafedél",
  "leiras": "Az aknafedél eltört, balesetveszélyes az úttesten.",
  "helyszin": "Kossuth utca 12., Budapest",
  "szelesseg": 47.4984,
  "hosszusag": 19.0402,
  "kategoria": "Uthiba",
  "bekuldes": "2026-11-03",
  "ellenorzes": "2026-11-04",
  "allapot": "Folyamatban",
  "veszelyes": false,
  "hatarido": "2026-11-25",
  "lezaras": null,
  "ertekeles": null,
  "foAzonosito": null,
  "indok": null,
  "kepek": ["betort_aknafedel.jpg"],
  "reszfeladatok": [
    {
      "szervezet": "Utfenntartas",
      "cim": "Aknafedél cseréje",
      "hatarido": "2026-11-20",
      "kesz": true,
      "megjegyzes": "Kicserélve.",
      "elotte": "betort_aknafedel_r1_elotte.jpg",
      "utana": "betort_aknafedel_r1_utana.jpg"
    }
  ]
}
```

- **kategoria:** `Egyeb`, `Uthiba`, `Kozvilagitas`, `Hulladek`, `Utcabutor`, `Forgalomtechnika`, `Parkok`, `ElhagyottJarmu`, `Kozmu`
- **allapot:** `Uj`, `Ellenorizve`, `Folyamatban`, `LezarasraVar`, `Lezart`, `Teves`, `HataskoronKivuli`, `Osszevont`
- **szervezet:** `Utfenntartas`, `Koztisztasag`, `Kozvilagitas`
- Az `azonosito` és a `kovetokod` mezőket a szerver generálja, a `foAzonosito` az összevont ügy főbejelentésének azonosítója.

A mintaadatok betöltése az alkalmazásba még nem készült el.
