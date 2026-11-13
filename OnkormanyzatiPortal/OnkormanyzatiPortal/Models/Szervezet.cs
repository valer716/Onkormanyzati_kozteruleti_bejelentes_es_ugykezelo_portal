using System.ComponentModel.DataAnnotations;

namespace OnkormanyzatiPortal.Models;

public class Szervezet
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Nev { get; set; } = string.Empty;
}