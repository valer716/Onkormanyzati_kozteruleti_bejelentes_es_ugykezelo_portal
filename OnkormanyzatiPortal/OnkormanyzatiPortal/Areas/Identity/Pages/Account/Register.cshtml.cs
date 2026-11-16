using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OnkormanyzatiPortal.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class RegisterModel(
    UserManager<IdentityUser> felhasznaloKezelo,
    SignInManager<IdentityUser> belepesKezelo) : PageModel
{
    [BindProperty]
    public RegisztracioAdatok Adatok { get; set; } = new();

    public string? ReturnUrl { get; set; }

    public class RegisztracioAdatok
    {
        [Required(ErrorMessage = "Add meg az e-mail-címet.")]
        [EmailAddress(ErrorMessage = "Érvénytelen e-mail-cím.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Add meg a jelszót.")]
        [DataType(DataType.Password)]
        public string Jelszo { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Compare(nameof(Jelszo), ErrorMessage = "A két jelszó nem egyezik.")]
        public string JelszoUjra { get; set; } = string.Empty;
    }

    public void OnGet(string? returnUrl = null)
    {
        ReturnUrl = returnUrl;
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        ReturnUrl = returnUrl;

        if (!ModelState.IsValid)
            return Page();

        var felhasznalo = new IdentityUser { UserName = Adatok.Email, Email = Adatok.Email };
        var eredmeny = await felhasznaloKezelo.CreateAsync(felhasznalo, Adatok.Jelszo);

        if (!eredmeny.Succeeded)
        {
            foreach (var hiba in eredmeny.Errors)
                ModelState.AddModelError(string.Empty, hiba.Description);

            return Page();
        }

        if (felhasznaloKezelo.Options.SignIn.RequireConfirmedAccount)
            return RedirectToPage("./Login");

        await belepesKezelo.SignInAsync(felhasznalo, isPersistent: false);
        return LocalRedirect(Url.IsLocalUrl(returnUrl) ? returnUrl! : "~/");
    }
}
