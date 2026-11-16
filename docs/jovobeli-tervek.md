# Jövőbeli tervek

A feladat "Riportok és bővítések" részéhez kapcsolódó ötletek, amelyek az alapkövetelményeken túlmutatnak, ezért a beadandóban nem készülnek el.

## Helymeghatározás

- Cím kitöltése a térképen megjelölt pontból egy geokódoló szolgáltatással (például OpenStreetMap Nominatim).
- A javasolt címet a lakos át tudja írni, mert a térképi adat nem mindig pontos (például lakásszám vagy új épület). A pontot is lehessen áthelyezni.

## Integráció

- Napi vagy heti JSON-export a városüzemeltetési rendszer felé: a teljes bejelentés (cím, leírás, helyszín, koordináta, kategória, állapot, időpontok), a bejelentő adatai nélkül, mert azokat egyébként sem tároljuk.
- Az export dátum szerint szűrhető legyen, hogy az adott nap beérkezései lekérhetők legyenek.

## Vezetői kimutatás

- Területi sűrűség irányítószám szerinti bontásban, térképes megjelenítéssel.
- A beérkezés és a lezárás ideje a vezetői listán és az exportban is jelenjen meg.
- Duplikációgyanú jelzése azonos kategóriájú, közeli bejelentéseknél.
- Az érkezett bejelentések száma időszak szerint (7 nap, 30 nap, év), az időszak váltható.
- A legfrissebb bejelentések listája a vezetői oldalon.

## Ismert korlátok

- A követőkód 6 számjegyű véletlen szám, hogy megjegyezhető legyen. Aki ismeri, látja az ügy állapotát és határidejét, ezért találgatható. Éles rendszerben hosszabb vagy lejáró kód, illetve a próbálkozások korlátozása kellene.
- A bejelentő szabad szövegbe személyes adatot is írhat (név, rendszám), és a fotón is lehet felismerhető személy. Ezt a portál jelenleg nem szűri.
- A lakos nem kap értesítést az állapotváltozásról, a követés oldalon kell újra megnéznie az ügyet.
