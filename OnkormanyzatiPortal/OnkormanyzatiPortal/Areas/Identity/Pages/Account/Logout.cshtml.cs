using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OnkormanyzatiPortal.Areas.Identity.Pages.Account;

public class LogoutModel(SignInManager<IdentityUser> belepesKezelo) : PageModel
{
    public IActionResult OnGet()
    {
        return LocalRedirect("~/");
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        await belepesKezelo.SignOutAsync();
        return LocalRedirect(Url.IsLocalUrl(returnUrl) ? returnUrl! : "~/");
    }
}
