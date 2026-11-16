# Önkormányzati közterületi bejelentés- és ügykezelő portál

ASP.NET Core MVC (.NET 10) webalkalmazás, amelyben a lakosok közterületi problémákat jelenthetnek be, az önkormányzat pedig kezeli és követi az ügyeket.

- A lakos belépés nélkül, névtelenül küld be bejelentést (cím, leírás, helyszín, térképes pont, fotó), és egy követőkóddal nézheti az ügy állapotát.
- Az ügyintéző ellenőrzi az ügyet, határidőt állít, részfeladatot ad a felelős szervezetnek, összevon, és lezár.
- A végrehajtó a saját szervezete részfeladatait látja, helyszíni megjegyzést és előtte/utána fotót tölt fel.
- A vezető kimutatást, térképet és összesített JSON-exportot lát.
- Minden állapotváltás naplózásra kerül.

## Mi kell a futtatáshoz

- Windows, .NET 10 SDK
- SQL Server LocalDB (a Visual Studio telepítőjében az "SQL Server Express LocalDB" komponens)
- `dotnet-ef` eszköz: `dotnet tool install --global dotnet-ef`

## Indítás

A legegyszerűbb a gyökérben lévő `Inditas.bat`: létrehozza az adatbázist, elindítja a portált és megnyitja a böngészőt a `http://localhost:5131` címen.

Kézzel:

```
dotnet ef database update --project OnkormanyzatiPortal/OnkormanyzatiPortal
dotnet run --project OnkormanyzatiPortal/OnkormanyzatiPortal --launch-profile http
```

Visual Studióban az `OnkormanyzatiPortal.slnx` megnyitása után F5 (a címek: `https://localhost:7134`, `http://localhost:5131`).

A portál a saját gépen fut, kívülről nem érhető el.

## Mintaadatok és demó-fiókok

Fejlesztői módban az első indításkor a portál betölti a `MintaAdatok` mappa tartalmát (538 győri bejelentés képekkel, naplóval és részfeladatokkal), és létrehozza a demó-fiókokat. A bejelentéseket csak akkor tölti be, ha az adatbázis még üres, a már meglévő fiókokat és adatokat nem módosítja. Kikapcsolható: `Fejlesztes:MintaAdatBetoltes` = `false` (`appsettings.Development.json` vagy User Secrets).

| Szerepkör | E-mail | Szervezet |
|---|---|---|
| Ügyintéző | erika.ugyintezo@portal.test | |
| Ügyintéző | gabor.ugyintezo@portal.test | |
| Ügyintéző | nora.ugyintezo@portal.test | |
| Vezető | bela.vezeto@portal.test | |
| Végrehajtó | balint.utfenntartas@portal.test | Útfenntartás |
| Végrehajtó | kata.koztisztasag@portal.test | Köztisztaság |
| Végrehajtó | peter.kozvilagitas@portal.test | Közvilágítás |

Jelszó mindegyikhez: `Demo!2026`. Ezek kitalált, csak helyi tesztfiókok.

Új adatbázissal kezdeni így lehet: `dotnet ef database drop --project OnkormanyzatiPortal/OnkormanyzatiPortal`, majd újraindítás.

## Saját fiók szerepkörrel

Ha a saját e-mail-címeddel szeretnél belépni, regisztrálj a portálon, majd a User Secrets-ben (Visual Studióban a projekten jobb klikk, Manage User Secrets) add meg:

```json
{
  "Fejlesztes:UgyintezoEmail": "a-te-cimed@pelda.hu",
  "Fejlesztes:VezetoEmail": "masik-cim@pelda.hu",
  "Fejlesztes:Vegrehajtok": [
    { "Email": "harmadik-cim@pelda.hu", "SzervezetId": 1 }
  ]
}
```

A szervezetek: 1 Útfenntartás, 2 Köztisztaság, 3 Közvilágítás. Újraindítás után kapják meg a szerepkört. A User Secrets a saját gépen marad, nem kerül a repóba.

## Mappák

| Mappa | Tartalom |
|---|---|
| `OnkormanyzatiPortal` | A webalkalmazás forráskódja |
| `docs` | Adatmodell, jogosultsági mátrix, tesztesetek, jövőbeli tervek |
| `MintaAdatok` | Mintabejelentések, képek és a mintabejelentés-generátor |

## Ismert korlátok

- A követőkód egyszerű, véletlen azonosító. Aki ismeri, látja az ügy állapotát és határidejét.
- A lakos nem kap értesítést az állapotváltozásról, a követés oldalon kell újra megnéznie.
- A portál nem szűri a szabad szövegbe vagy fotóra került személyes adatot.
- Nincs spamvédelem a bejelentés beküldésénél.
