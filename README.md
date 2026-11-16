# Önkormányzati közterületi bejelentés- és ügykezelő portál

ASP.NET Core MVC (.NET 10) webalkalmazás közterületi bejelentések beküldésére és kezelésére. A lakos belépés nélkül, névtelenül küld be bejelentést, az ügyintéző, a végrehajtó és a vezető belépve dolgozik, az állapotváltások naplózásra kerülnek.

## Futtatás

Szükséges (Windows):

- **.NET 10 SDK**: dotnet.microsoft.com/download
- **SQL Server LocalDB**: a Visual Studio telepítőjében (Visual Studio Installer, Egyedi összetevők, "SQL Server Express LocalDB"), vagy az SQL Server Express telepítőjével
- **dotnet-ef eszköz**: parancssorban `dotnet tool install --global dotnet-ef` (az `Inditas.bat` ezt is elvégzi)

Indítás: dupla kattintás az `Inditas.bat` fájlra. Egy parancssori ablak nyílik meg, ez futtatja a webszervert (külön webszervert telepíteni nem kell, az alkalmazás tartalmazza), létrehozza az adatbázist, és megnyitja a böngészőt a `http://localhost:5131` címen. Leállítás: az ablak bezárása vagy Ctrl+C.

Visual Studióban az `OnkormanyzatiPortal.slnx` megnyitása után F5.

## Mintaadatok

Első indításkor a portál betölti a `MintaAdatok` mappa győri mintabejelentéseit, és létrehozza a demó-fiókokat (ügyintéző, végrehajtó, vezető). Ha az adatbázis már tartalmaz bejelentést, nem tölt be újra. Kikapcsolás: `Fejlesztes:MintaAdatBetoltes` = `false` az `appsettings.Development.json` fájlban.
