namespace OnkormanyzatiPortal.Models;

public record VezetoiUgySor(
    int Id,
    string Cim,
    string Helyszin,
    BejelentesKategoria Kategoria,
    BejelentesAllapot Allapot,
    DateTime LetrehozvaUtc,
    DateTime? Hatarido,
    bool Lejart,
    int NyitvaNap,
    double? Szelesseg,
    double? Hosszusag,
    int? FotoId);

public record VezetoiHaviSor(string Honap, int Darab);

public record VezetoiTerulet(string Iranyitoszam, int Osszes, int Nyitott, int Lejart);

public record VezetoiKategoriaSor(
    BejelentesKategoria Kategoria,
    int Osszes,
    int Nyitott,
    int Lezart,
    int Egyeb,
    double? AtlagosMegoldasiNap);

public record VezetoiKimutatasNezet(
    IReadOnlyList<VezetoiUgySor> NyitottUgyek,
    double? AtlagosMegoldasiNap,
    IReadOnlyList<VezetoiKategoriaSor> Kategoriak,
    IReadOnlyDictionary<BejelentesAllapot, int> Allapotok,
    IReadOnlyList<VezetoiHaviSor> Havi,
    IReadOnlyList<VezetoiTerulet> Teruletek,
    int OsszesUgy,
    int LezartUgy);
