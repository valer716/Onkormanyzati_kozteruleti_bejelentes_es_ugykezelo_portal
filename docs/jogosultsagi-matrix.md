# Szerepkör- és jogosultsági mátrix

A szerepkörök: `Ugyintezo`, `Vegrehajto`, `Vezeto`. A lakos és a vendég nem lép be. A jogosultságot a kontrollerek `[Authorize(Roles = "...")]` attribútumai érvényesítik, a szerver minden kérésnél ellenőriz.

## Mátrix

| Művelet | Vendég (lakos) | Ügyintéző | Végrehajtó | Vezető |
|---|---|---|---|---|
| Bejelentés beküldése (űrlap, térkép, fotó) | Igen | Igen | Igen | Igen |
| Állapot lekérése követőkóddal | Igen | Igen | Igen | Igen |
| Értékelés beküldése lezárt ügyre (követőkóddal) | Igen | Igen | Igen | Igen |
| Ügylista, keresés, szűrés | Nem | Igen | Nem | Nem |
| Ügy részletei, napló, fotók megtekintése | Nem | Igen | Csak a saját szervezete feladataihoz tartozó ügyek részletei és fotói | Igen, csak olvasható |
| Ellenőrzés, adatpontosítás, határidő megadása | Nem | Igen | Nem | Nem |
| A bejelentés címének, leírásának, helyszínének és kategóriájának javítása (lezárásig) | Nem | Igen | Nem | Nem |
| Részfeladat kiosztása szervezetnek | Nem | Igen | Nem | Nem |
| Veszélyes jelölés, összevonás, téves, hatáskörön kívüli | Nem | Igen | Nem | Nem |
| Ügy lezárása | Nem | Igen | Nem | Nem |
| Saját szervezet részfeladatainak listája | Nem | Nem | Igen | Nem |
| Helyszíni megjegyzés, előtte/utána fotó, készre jelentés | Nem | Nem | Igen, csak saját | Nem |
| Vezetői kimutatás, diagramok, szűrhető térkép és ügytáblázat | Nem | Nem | Nem | Igen |
| Összesített JSON-export | Nem | Nem | Nem | Igen |
| Ügy vagy napló törlése | Nem | Nem | Nem | Nem |
| Szerepkör módosítása az alkalmazásból | Nem | Nem | Nem | Nem |

## Megjegyzések

- Az ügyintéző a bejelentés címét, leírását, helyszínét és kategóriáját a lezárásig javíthatja (Szerkesztés gomb), a módosítás bekerül a státusztörténetbe. A lakos a beküldés után nem módosíthat.

- Az ügyintéző az önkormányzat hivatali dolgozója, ő a portál központi kezelője: ő dönt a téves és a hatáskörön kívüli ügyekről, indoklással. A végrehajtó a feladatot végző szervezet munkatársa, a vezető csak olvas és kimutat, ügyet nem módosít. A szerepkörök nem épülnek egymásra, mindegyiknek más a feladata.
- A lakos nem regisztrál és nem lép be, névtelen. A portál belépési és regisztrációs oldalai a dolgozók fiókjaihoz tartoznak.
- A követőkód a bejelentő titka. Aki ismeri, látja az ügy állapotát és határidejét, mást nem. Összevont ügynél a főügy állapota látszik.
- A végrehajtó csak a saját szervezetéhez tartozó részfeladatot módosíthatja, idegen feladat azonosítójára hibát kap.
- A végrehajtó feladata csak `Folyamatban` állapotú ügynél és csak elkészülés előtt módosítható.
- A szerepköröket induláskor az alkalmazás hozza létre. Fejlesztői környezetben a `Fejlesztes:` beállítások (`UgyintezoEmail`, `VezetoEmail`, `Vegrehajtok`) rendelik a szerepkört és a szervezetet a megadott e-mail-című felhasználókhoz.
- Az export személyes adatot nem tartalmaz, csak kategória és állapot szerinti darabszámokat.
