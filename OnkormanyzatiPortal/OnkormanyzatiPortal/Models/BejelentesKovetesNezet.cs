namespace OnkormanyzatiPortal.Models;

public record BejelentesKovetesNezet(
    int Id,
    Guid Kovetokod,
    BejelentesAllapot Allapot,
    DateTime? Hatarido,
    int? Ertekeles);