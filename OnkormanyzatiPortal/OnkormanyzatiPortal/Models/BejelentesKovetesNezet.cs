namespace OnkormanyzatiPortal.Models;

public record KovetesEsemeny(DateTime IdopontUtc, BejelentesAllapot Allapot);

public record BejelentesKovetesNezet(
    int Id,
    Guid Kovetokod,
    BejelentesAllapot Allapot,
    DateTime? Hatarido,
    int? Ertekeles,
    IReadOnlyList<KovetesEsemeny>? Esemenyek = null);
