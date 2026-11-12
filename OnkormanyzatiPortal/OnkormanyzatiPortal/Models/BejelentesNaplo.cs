using System.ComponentModel.DataAnnotations;

namespace OnkormanyzatiPortal.Models;

public class BejelentesNaplo
{
    public int Id { get; set; }

    public int BejelentesId { get; set; }
    public Bejelentes Bejelentes { get; set; } = null!;

    [StringLength(100)]
    public string Muvelet { get; set; } = string.Empty;

    public BejelentesAllapot? ElozoAllapot { get; set; }
    public BejelentesAllapot UjAllapot { get; set; }
    public DateTime? Hatarido { get; set; }

    public DateTime IdopontUtc { get; set; } = DateTime.UtcNow;

    public string? FelhasznaloId { get; set; }
}