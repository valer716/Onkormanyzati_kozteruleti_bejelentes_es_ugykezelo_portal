using System.ComponentModel.DataAnnotations;

namespace OnkormanyzatiPortal.Models;

public class Reszfeladat
{
    public int Id { get; set; }

    public int BejelentesId { get; set; }
    public Bejelentes Bejelentes { get; set; } = null!;

    public int SzervezetId { get; set; }
    public Szervezet Szervezet { get; set; } = null!;

    [Required, StringLength(160)]
    public string Cim { get; set; } = string.Empty;

    public DateTime Hatarido { get; set; }
    public bool Elkeszult { get; set; }
    [StringLength(1000)]
    public string? Megjegyzes { get; set; }
}