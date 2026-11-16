# Jövőbeli tervek

Ez a lista azt tartalmazza, ami a feladat alapkövetelményein túl hasznos lenne, de a beadandóban nem készül el. Részben a feladat bővítési lehetőségeihez kapcsolódik, részben a csapat ötlete. A fájl célja, hogy látszódjon, tudunk a hiányosságokról, és mi lenne a következő lépés.

## Lakossági bejelentés

| Terv | Miért kell | Egyszerű megoldás |
|---|---|---|
| Botvédelem a beküldéshez | A bejelentés belépés nélkül küldhető, ezért automatikusan is tele lehet spamelni | CAPTCHA, rejtett csapdamező az űrlapon |
| Beküldések korlátozása | Egy cím ne tudjon korlátlanul beküldeni | Kérésszám-korlát IP-címenként, például napi 50 bejelentés (az ASP.NET Core beépített rate limiter-ével) |
| Saját kódok megjegyzése | A lakosnak most fel kell írnia a követőkódot | A böngésző helyi tárolójában megőrzött kódok, „Saját bejelentéseim" oldal |
| Értesítés az állapotváltozásról | A lakos csak akkor tudja meg, hogy megoldódott az ügye, ha újra megnézi | Opcionális e-mail-cím megadása, de ez személyes adat, ezért csak külön döntés után |

## Követés és adatvédelem

- A követőkód most egy egyszerű, véletlen `Guid`. Aki ismeri, látja az ügy állapotát és határidejét. Éles rendszerben érdemes lejáró vagy aláírt tokenre cserélni.
- A bejelentő szabad szövegbe személyes adatot is írhat (név, rendszám), és a fotón is lehet felismerhető személy. Ezt a portál jelenleg nem szűri.

## Helymeghatározás

- Cím kitöltése a térképen megjelölt pontból egy geokódoló szolgáltatással (például OpenStreetMap Nominatim).
- A javasolt címet a lakos át tudja írni, mert a térképi adat nem mindig pontos (például lakásszám vagy új épület). A pontot is lehessen áthelyezni.
- Jelölés arról, hogy a cím kézzel módosult.

## Integráció

- Napi vagy heti JSON-export a városüzemeltetési rendszer felé. Teljes bejelentés (cím, leírás, helyszín, koordináta, kategória, állapot, időpontok), a bejelentő adatai nélkül, mert azokat egyébként sem tároljuk. A képek külön hivatkozással vagy letöltéssel kerülnének át.
- Az exportnak dátum szerint szűrhetőnek kell lennie, hogy az adott nap beérkezései lekérhetők legyenek.

## Vezetői kimutatás

- Területi sűrűség irányítószám szerinti bontásban, térképes megjelenítéssel.
- A beérkezés és a lezárás ideje a vezetői listán és az exportban is jelenjen meg.
- Duplikációgyanú jelzése azonos kategóriájú, közeli bejelentéseknél.

## Bemutatás

- Szerepváltó fejléc fejlesztői módban, amellyel egy kattintással át lehet lépni egyik demó-fiókból a másikba. A jogosultságok kipróbálását és a bemutatót gyorsítaná.
- A portál külső elérhetősége a bemutatóra (például tunnel a helyi gép elé), a végleges tárhely külön kérdés, mert a .NET alkalmazás szervert igényel.
