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
    double? Hosszusag);

public record VezetoiHaviSor(string Honap, int Darab);

public record VezetoiTerulet(string Iranyitoszam, int Osszes, int Nyitott, int Lejart);

public record VezetoiKimutatasNezet(
    IReadOnlyList<VezetoiUgySor> NyitottUgyek,
    double? AtlagosMegoldasiNap,
    IReadOnlyDictionary<BejelentesKategoria, int> Kategoriak,
    IReadOnlyDictionary<BejelentesAllapot, int> Allapotok,
    IReadOnlyList<VezetoiHaviSor> Havi,
    IReadOnlyList<VezetoiTerulet> Teruletek,
    int OsszesUgy,
    int LezartUgy);
