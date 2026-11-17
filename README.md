# Önkormányzati közterületi bejelentés- és ügykezelő portál

ASP.NET Core MVC (.NET 10) webalkalmazás közterületi bejelentések beküldésére és kezelésére. A lakos belépés nélkül, névtelenül küld be bejelentést, az ügyintéző, a végrehajtó és a vezető belépve dolgozik, az állapotváltások naplózásra kerülnek.

## Technológiák

| Mire | Mit használunk |
|---|---|
| Webalkalmazás | ASP.NET Core MVC (.NET 10), Razor nézetek |
| Adatbázis | SQL Server LocalDB, Entity Framework Core migrációkkal |
| Belépés, szerepkörök | ASP.NET Core Identity (ügyintéző, végrehajtó, vezető) |
| Felület | Bootstrap 5 |
| Térkép | Leaflet és OpenStreetMap |
| Minta és betöltés | JSON és képek a `MintaAdatok` mappában, a portál indításkor betölti |

## Külső könyvtárak és szolgáltatások

A térkép és a diagramok az internetről töltődnek be, ezért ezekhez a nézetekhez internetkapcsolat kell.

| Mit használunk | Mire | Melyik oldalon |
|---|---|---|
| OpenStreetMap csempék (tile.openstreetmap.org) | a térkép képe; csak a megjelenített terület kérése megy ki, bejelentési adat nem | Új bejelentés, Vezetői kimutatás |
| Leaflet | térkép megjelenítése, pont kijelölése | Új bejelentés, Vezetői kimutatás |
| Leaflet.markercluster | közeli ügyek összevonása számmal jelölt körökké | Vezetői kimutatás |
| Chart.js | oszlop- és kördiagramok | Vezetői kimutatás |
| Bootstrap, jQuery (a projekttel együtt) | felület, űrlapellenőrzés | minden oldal |

## Oldalak

| Oldal | Ki éri el | Mit csinál |
|---|---|---|
| Kezdőlap | mindenki | üdvözlő oldal a logóval |
| Új bejelentés | mindenki | űrlap térképpel és fényképpel, beküldés után 6 számjegyű követőkód |
| Ügykövetés | mindenki | a követőkóddal az ügy állapota, határideje és története, lezárás után értékelés |
| Belépés, Regisztráció | mindenki | dolgozói fiókokhoz |
| Ügyintézés | ügyintéző | az ügyek listája rendezéssel és szűréssel |
| Ügy részletei | ügyintéző | adatok javítása, ellenőrzés és határidő, feladat kiosztása, összevonás, veszélyes jelölés, lezárás |
| Munkafeladatok | végrehajtó | a saját szervezet feladatai kártyákon, megjegyzés és fotó rögzítése, készre jelentés |
| Vezetői kimutatás | vezető | számok, diagramok, szűrhető térkép és ügytáblázat, irányítószám szerinti bontás |
| Ügy előnézete | vezető | az ügy adatainak, fotóinak és történetének megtekintése, módosítás nélkül |

## Futtatás

Szükséges (Windows):

- **.NET 10 SDK**: dotnet.microsoft.com/download
- **SQL Server LocalDB**: a Visual Studio telepítőjében (Visual Studio Installer, Egyedi összetevők, "SQL Server Express LocalDB"), vagy az SQL Server Express telepítőjével
- **dotnet-ef eszköz**: parancssorban `dotnet tool install --global dotnet-ef` (az `Inditas.bat` ezt is elvégzi)

Indítás: dupla kattintás az `Inditas.bat` fájlra. Egy parancssori ablak nyílik meg, ez futtatja a webszervert (külön webszervert telepíteni nem kell, az alkalmazás tartalmazza), létrehozza az adatbázist, és megnyitja a böngészőt a `http://localhost:5131` címen. Leállítás: az ablak bezárása vagy Ctrl+C.

Visual Studióban az `OnkormanyzatiPortal.slnx` megnyitása után F5.

## Belépés

A bejelentés beküldéséhez és a követéshez nem kell belépni. A dolgozói felületekhez a demó-fiókok (jelszó mindegyikhez: `Sze!2026`):

| Szerepkör | E-mail | Szervezet |
|---|---|---|
| Ügyintéző | erika.ugyintezo@portal.test | |
| Ügyintéző | gabor.ugyintezo@portal.test | |
| Ügyintéző | nora.ugyintezo@portal.test | |
| Vezető | bela.vezeto@portal.test | |
| Végrehajtó | balint.utfenntartas@portal.test | Útfenntartás |
| Végrehajtó | kata.koztisztasag@portal.test | Köztisztaság |
| Végrehajtó | peter.kozvilagitas@portal.test | Közvilágítás |
| Végrehajtó | zsolt.kozteruletfelugyelet@portal.test | Közterület-felügyelet |
| Végrehajtó | anna.kozmuvek@portal.test | Közművek |

## Mintaadatok

Első indításkor a portál betölti a `MintaAdatok` mappa győri mintabejelentéseit, és létrehozza a demó-fiókokat (ügyintéző, végrehajtó, vezető). A meglévő fiókokhoz és bejelentésekhez csak hozzáad, a már betöltött mintákat kihagyja. Kikapcsolás: `Fejlesztes:MintaAdatBetoltes` = `false` az `appsettings.Development.json` fájlban.
