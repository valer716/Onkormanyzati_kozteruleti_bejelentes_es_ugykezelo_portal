using System.ComponentModel.DataAnnotations;

namespace OnkormanyzatiPortal.Models;

public enum BejelentesKategoria
{
    [Display(Name = "Egyéb")] Egyeb = 0,
    [Display(Name = "Kátyú / úthiba")] Uthiba = 1,
    [Display(Name = "Közvilágítás")] Kozvilagitas = 2,
    [Display(Name = "Illegális hulladék")] Hulladek = 3,
    [Display(Name = "Sérült utcabútor")] Utcabutor = 4
}