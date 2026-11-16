namespace OnkormanyzatiPortal.Models;

public record FoUgyJelolt(
    int Id,
    string Cim,
    string Helyszin,
    double? Szelesseg,
    double? Hosszusag,
    double? TavolsagMeter);
