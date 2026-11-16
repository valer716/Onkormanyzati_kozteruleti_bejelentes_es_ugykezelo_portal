# A feladat és az állapota

A 9. feladat: Önkormányzati közterületi bejelentés- és ügykezelő portál. Az eredeti szöveg képként: [feladat.png](feladat.png).

Jelölések: **KÉSZ**, **RÉSZBEN**, **HIÁNYZIK**, **NEM KÖTELEZŐ** (a szöveg csak lehetőségként említi).

## Elvárt működés

| Követelmény | Állapot |
|---|---|
| A lakos térképi hely, kategória, leírás és fénykép megadásával küld bejelentést, akár regisztráció nélkül | **KÉSZ** |
| Az ügyintéző ellenőrzi, pontosítja, összevonja a hasonló eseteket, felelős szervezethez irányítja, határidőt állít | **KÉSZ** (a felület átnézése hátra van) |
| A végrehajtó munkafeladatot kap, helyszíni megjegyzést és előtte-utána fényképet tölt fel | **KÉSZ** |
| A lezárást az ügyintéző ellenőrzi, a bejelentő visszajelzést adhat, személyes adata nem jelenik meg a nyilvános nézetben | **KÉSZ** (értékelés, a követés oldal csak állapotot és határidőt mutat) |
| Téves, hatáskörön kívüli, veszélyes és tömegesen jelzett ügyek kezelése, minden lényeges állapotváltozás naplózása | **KÉSZ** |

## Adatok és integráció

| Követelmény | Állapot |
|---|---|
| Tárolás: bejelentések, helyek, kategóriák, csatolmányok, szervezetek, feladatok, határidők, státusztörténet, visszajelzések | **KÉSZ** |
| Térképen megjelenő ügyek | **KÉSZ** |
| Geokódoló vagy térképi API a koordinátából képzett címhez | **NEM KÖTELEZŐ**, nincs meg |
| Anonimizált JSON-adatcsere külső városüzemeltetési rendszer felé | **NEM KÖTELEZŐ**, az összesített export megvan, a teljes bejelentés-export nincs |

## Riportok és bővítések

| Követelmény | Állapot |
|---|---|
| Térképes és táblázatos vezetői nézet a nyitott ügyekről | **KÉSZ** |
| Átlagos megoldási idő | **KÉSZ** |
| Határidő-túllépés | **KÉSZ** |
| Kategóriánkénti gyakoriság | **KÉSZ** |
| Területi sűrűség | **HIÁNYZIK** |
| Bejelentés több részfeladatból, csak mindegyik után zárható | **KÉSZ** |
| Bővítések (duplikációgyanú, körzetfelelős értesítés, nyilvános adatportál, elégedettségi mutató) | **NEM KÖTELEZŐ**, nincsenek meg |

## Projektminimum

| Követelmény | Állapot |
|---|---|
| Áttekinthető, reszponzív felület | **RÉSZBEN** (telefonos és tabletes ellenőrzés hátra van) |
| Legalább három szerepkör, többlépéses folyamat, keresés, szűrés, adatvalidáció, vezetői kimutatás | **KÉSZ** |
| Mintaadatok | **KÉSZ** |
| Dokumentált tesztesetek | **RÉSZBEN** (a tesztesetek le vannak írva, az automata tesztek és az eredmények hiányoznak) |
| Adatmodell, szerepkör- és jogosultsági mátrix | **KÉSZ** |
| Legalább egy import, export vagy külső API | **KÉSZ** (mintaadat-betöltés, JSON-export, térkép) |
| Naplózás, hibás vagy hiányzó adatok kezelése | **KÉSZ** (a napló), **RÉSZBEN** (a hiányzó koordináta még nem kötelező) |

## Tervezett kiegészítések

Ezek nem szerepelnek a feladat szövegében:

- vezetői összesítés az érkezett bejelentésekről időszak szerint (7 nap, 30 nap, év)
- a legfrissebb bejelentések listája a vezetői oldalon
