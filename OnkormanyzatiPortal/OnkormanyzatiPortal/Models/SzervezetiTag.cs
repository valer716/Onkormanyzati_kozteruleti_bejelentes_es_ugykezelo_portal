using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace OnkormanyzatiPortal.Models;

public class SzervezetiTag
{
    [Key]
    public string FelhasznaloId { get; set; } = string.Empty;
    public IdentityUser Felhasznalo { get; set; } = null!;

    public int SzervezetId { get; set; }
    public Szervezet Szervezet { get; set; } = null!;
}