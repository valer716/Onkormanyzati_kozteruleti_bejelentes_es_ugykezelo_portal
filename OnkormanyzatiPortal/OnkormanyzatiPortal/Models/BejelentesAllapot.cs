using System.ComponentModel.DataAnnotations;

namespace OnkormanyzatiPortal.Models;

public enum BejelentesAllapot
{
    [Display(Name = "Új")] Uj = 0,
    [Display(Name = "Ellenőrizve")] Ellenorizve = 1,
    [Display(Name = "Folyamatban")] Folyamatban = 2,
    [Display(Name = "Lezárásra vár")] LezarasraVar = 3,
    [Display(Name = "Lezárt")] Lezart = 4
}