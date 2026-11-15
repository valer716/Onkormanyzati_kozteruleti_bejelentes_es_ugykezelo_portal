# Tesztesetek

A teszteknek beszédes nevük van: `Művelet_Elvárás_Feltétel`, így a névből kiderül, mit ellenőriznek. Az eredmény oszlopban **Nincs futtatva** szerepel, amíg a tesztet ténylegesen le nem futtattuk. Csak a lefuttatott teszt jelölhető sikeresnek.

Az automatikus tesztek helye a tervezett `OnkormanyzatiPortal.Tests` projekt (futtatás: `dotnet test`). A kézi tesztek a böngészőben, a mintaadatokkal próbálhatók ki.

## Bejelentés és követés

| Azonosító | Teszt neve | Mit ellenőriz | Elvárt eredmény | Típus | Eredmény |
|---|---|---|---|---|---|
| T01 | Letrehozas_Elmenti_ErvenyesAdatokkal | Vendég érvényes adatokkal beküld | Mentés, követőkód megjelenik, kezdő állapot Uj | automata | Nincs futtatva |
| T02 | Letrehozas_Elutasitja_UresVagyTulHosszuCimet | Cím hossza (kötelező, legfeljebb 120) | Hibaüzenet, nincs mentés | automata | Nincs futtatva |
| T03 | Letrehozas_Elutasitja_ErvenytelenKoordinatat | Szélesség −90…90, hosszúság −180…180 | Hibaüzenet, nincs mentés | automata | Nincs futtatva |
| T04 | Fotofeltoltes_Elutasitja_NemJpegVagyPngFajlt | A fájl tartalmának ellenőrzése | Hiba, a kiterjesztés nem számít | automata | Nincs futtatva |
| T05 | Fotofeltoltes_Elutasitja_5MegabajtFelettiFajlt | Méretkorlát | Hiba | automata | Nincs futtatva |
| T06 | Kovetes_NemTalalhato_IsmeretlenKoddal | Ismeretlen követőkód | „Nem található bejelentés" | automata | Nincs futtatva |
| T07 | Kovetes_CsakAllapototEsHataridotMutat | Szűkített lakossági nézet | Cím, leírás, helyszín nem szerepel a válaszban | automata | Nincs futtatva |
| T08 | Ertekel_CsakLezartUgyreMukodik | Értékelés csak lezárt ügyre, egyszer | Nem lezárt ügyre és második értékelésre hiba | automata | Nincs futtatva |

## Ügyintézői folyamat

| Azonosító | Teszt neve | Mit ellenőriz | Elvárt eredmény | Típus | Eredmény |
|---|---|---|---|---|---|
| T09 | Ellenoriz_Elutasitja_MultbeliHatarido | Határidő ma vagy később | Hibaüzenet | automata | Nincs futtatva |
| T10 | Ellenoriz_UjbolEllenorizveAllapotba_NaplozMuveletet | Állapotváltás és naplózás | Állapot Ellenorizve, napló sor régi és új állapottal | automata | Nincs futtatva |
| T11 | Kioszt_Elutasitja_AzUgyetMeghaladoHataridot | Részfeladat határideje legfeljebb az ügyé | Hibaüzenet | automata | Nincs futtatva |
| T12 | Kioszt_FolyamatbaAllitjaAzUgyet | Kiosztás hatása | Állapot Folyamatban, részfeladat létrejön | automata | Nincs futtatva |
| T13 | KulonlegesKezeles_Osszevon_CsakAzonosKategoriaval | Összevonás szabálya | Eltérő kategóriánál hiba | automata | Nincs futtatva |
| T14 | KulonlegesKezeles_Teves_IndokotKer | Téves és hatáskörön kívüli döntés | Indok nélkül hiba, legfeljebb 80 karakter | automata | Nincs futtatva |

## Végrehajtás és lezárás

| Azonosító | Teszt neve | Mit ellenőriz | Elvárt eredmény | Típus | Eredmény |
|---|---|---|---|---|---|
| T15 | Rogzit_MegjegyzesNelkulNemMenthet | Helyszíni megjegyzés kötelező | Hibaüzenet | automata | Nincs futtatva |
| T16 | Rogzit_IdegenSzervezetFeladatatNemModosithatja | Végrehajtó csak saját feladatot módosíthat | Nem található (404) | automata | Nincs futtatva |
| T17 | Rogzit_LezarasraVarAllapotbaVisz_HaMindenReszfeladatKesz | Készre jelentés hatása | Utolsó részfeladat után az ügy LezarasraVar | automata | Nincs futtatva |
| T18 | Lezar_NemZarhatoLe_HaVanKeszulatlanReszfeladat | Félkész lezárás tiltása | Hibaüzenet, az ügy nem Lezart | automata | Nincs futtatva |
| T19 | Lezar_NemZarhatoLe_HaNincsReszfeladat | Részfeladat nélküli lezárás tiltása | Hibaüzenet | automata | Nincs futtatva |
| T20 | Lezar_Lezarja_HaMindenReszfeladatKesz | Sikeres lezárás | Állapot Lezart, LezarvaUtc kitöltve, napló sor | automata | Nincs futtatva |
| T21 | Mentes_NemMenthetoElavultVerzioval | Párhuzamos módosítás | „Az ügy közben megváltozott" üzenet | automata | Nincs futtatva |

## Jogosultság és kimutatás

| Azonosító | Teszt neve | Mit ellenőriz | Elvárt eredmény | Típus | Eredmény |
|---|---|---|---|---|---|
| T22 | Jogosultsag_VendegNemLatjaAzUgylistat | Védett oldal vendégnek | Átirányítás a belépéshez | kézi | Nincs futtatva |
| T23 | Jogosultsag_VegrehajtoNemLatjaAVezetoiOldalt | Szerepkör szerinti tiltás | Hozzáférés megtagadva | kézi | Nincs futtatva |
| T24 | Vezetoi_AtlagosMegoldasiIdo_CsakLezartUgyekbol | Átlag számítása | Csak a Lezart ügyek számítanak | automata | Nincs futtatva |
| T25 | Vezetoi_Export_NemTartalmazSzemelyesAdatot | Export tartalma | Csak kategória, állapot, darabszám | kézi | Nincs futtatva |
| T26 | Vezetoi_Terkep_CsakKoordinatasUgyetMutat | Hiányzó koordináta kezelése | Koordináta nélküli ügy nem kerül a térképre | kézi | Nincs futtatva |
