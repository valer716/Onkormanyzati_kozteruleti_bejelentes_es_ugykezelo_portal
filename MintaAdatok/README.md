# Mintaadatok

Mintabejelentések a bemutatóhoz és a kipróbáláshoz.

| Elem | Mire való |
|---|---|
| `mintaadatok.json` | A mintabejelentések listája |
| `kepek/` | A bejelentésekhez tartozó fényképek |
| `Generator/` | Böngészős űrlap, amivel új mintabejelentés vehető fel |

## A generátor használata

1. Indítsd el a `Generator/Inditas.bat` fájlt (Python kell hozzá). Elindul egy kis helyi szerver, és megnyílik a `http://localhost:8081/Generator/index.html` oldal.
2. Az **Új bejelentés** fülön töltsd ki az űrlapot úgy, mintha a lakos küldené be: cím, leírás, helyszín, hely a térképen, kategória, fényképek. A **Mentés** a `mintaadatok.json` fájlhoz fűz egy elemet, a képeket a `kepek/` mappába teszi.
3. A **Szerkesztő** fülön látszanak a meglévő bejelentések, itt szerkeszthetők és törölhetők.

A szerver csak a saját gépről érhető el, és a fájlokat közvetlenül a `MintaAdatok` mappába írja. Bármelyik böngészővel használható.

## Egy bejelentés a fájlban

```json
{
  "cim": "Betört aknafedél",
  "leiras": "Az aknafedél eltört, balesetveszélyes.",
  "helyszin": "Kossuth utca 12., Budapest",
  "szelesseg": 47.4984,
  "hosszusag": 19.0402,
  "kategoria": "Uthiba",
  "bekuldes": "2026-11-03",
  "kepek": ["betort_aknafedel.jpg"]
}
```

A kategória értékei: `Egyeb`, `Uthiba`, `Kozvilagitas`, `Hulladek`, `Utcabutor`.

A mintaadatok betöltése az alkalmazásba még nem készült el.
