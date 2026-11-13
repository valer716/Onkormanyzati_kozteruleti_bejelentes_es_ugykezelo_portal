using System.ComponentModel.DataAnnotations;

namespace OnkormanyzatiPortal.Models;

public class Bejelentes
{
    public int Id { get; set; }

    [Required]
    [StringLength(120)]
    public string Cim { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Leiras { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Helyszin { get; set; } = string.Empty;

    public DateTime LetrehozvaUtc { get; set; } = DateTime.UtcNow;

    [EnumDataType(typeof(BejelentesKategoria))]
    public BejelentesKategoria Kategoria { get; set; } = BejelentesKategoria.Egyeb;

    public BejelentesAllapot Allapot { get; set; } = BejelentesAllapot.Uj;
    public DateTime? Hatarido { get; set; }
    [ConcurrencyCheck]
    public Guid Verzio { get; set; } = Guid.NewGuid();
    public DateTime? LezarvaUtc { get; set; }
}