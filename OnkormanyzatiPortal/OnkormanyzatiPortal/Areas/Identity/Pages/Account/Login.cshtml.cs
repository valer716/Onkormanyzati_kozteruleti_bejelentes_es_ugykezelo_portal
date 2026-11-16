using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OnkormanyzatiPortal.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class LoginModel(SignInManager<IdentityUser> belepesKezelo) : PageModel
{
    [BindProperty]
    public BelepesAdatok Adatok { get; set; } = new();

    public string? ReturnUrl { get; set; }

    public class BelepesAdatok
    {
        [Required(ErrorMessage = "Add meg az e-mail-címet.")]
        [EmailAddress(ErrorMessage = "Érvénytelen e-mail-cím.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Add meg a jelszót.")]
        [DataType(DataType.Password)]
        public string Jelszo { get; set; } = string.Empty;

        public bool Emlekezz { get; set; }
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

        var eredmeny = await belepesKezelo.PasswordSignInAsync(
            Adatok.Email, Adatok.Jelszo, Adatok.Emlekezz, lockoutOnFailure: false);

        if (eredmeny.Succeeded)
            return LocalRedirect(Url.IsLocalUrl(returnUrl) ? returnUrl! : "~/");

        ModelState.AddModelError(string.Empty, "Hibás e-mail-cím vagy jelszó.");
        return Page();
    }
}
