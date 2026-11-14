namespace OnkormanyzatiPortal.Models;

public record VezetoiUgySor(
    int Id,
    BejelentesKategoria Kategoria,
    BejelentesAllapot Allapot,
    DateTime? Hatarido,
    bool Lejart,
    double? Szelesseg,
    double? Hosszusag);

public record VezetoiKimutatasNezet(
    IReadOnlyList<VezetoiUgySor> NyitottUgyek,
    double? AtlagosMegoldasiNap,
    IReadOnlyDictionary<BejelentesKategoria, int> Kategoriak);