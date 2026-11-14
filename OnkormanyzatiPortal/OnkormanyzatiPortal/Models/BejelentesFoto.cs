using System.ComponentModel.DataAnnotations;

namespace OnkormanyzatiPortal.Models;

public enum BejelentesFotoTipus
{
    Bejelentes,
    Elotte,
    Utana
}

public class BejelentesFoto
{
    public int Id { get; set; }

    public int BejelentesId { get; set; }
    public Bejelentes Bejelentes { get; set; } = null!;

    public int? ReszfeladatId { get; set; }
    public Reszfeladat? Reszfeladat { get; set; }

    public BejelentesFotoTipus Tipus { get; set; }

    [StringLength(20)]
    public string TartalomTipus { get; set; } = string.Empty;

    public byte[] Tartalom { get; set; } = [];
}
public record FotoListaSor(int Id, BejelentesFotoTipus Tipus, int? ReszfeladatId);