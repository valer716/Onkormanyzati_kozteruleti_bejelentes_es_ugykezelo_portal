# Mintaadatok

Mintabejelentések a bemutatóhoz és a kipróbáláshoz.

| Elem | Mire való |
|---|---|
| `mintaadatok.json` | A mintabejelentések listája |
| `kepek/` | A bejelentésekhez tartozó fényképek |
| `Generator/` | Böngészős űrlap, amivel új mintabejelentés vehető fel |

## A generátor használata

1. Indítsd el az `Generator/Inditas.bat` fájlt (Python kell hozzá). Megnyitja a `http://localhost:8080/Generator/index.html` oldalt.
2. Töltsd ki az űrlapot úgy, mintha a lakos küldené be: cím, leírás, helyszín, hely a térképen, kategória, fényképek.
3. A **Mentés** gombbal válaszd ki ezt a `MintaAdatok` mappát. A generátor hozzáfűz egy elemet a `mintaadatok.json` fájlhoz, a képeket a `kepek/` mappába teszi.

A mentéshez Chrome vagy Edge kell. Más böngészőben a Letöltés gomb használható.

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
