using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnkormanyzatiPortal.Data;
using OnkormanyzatiPortal.Models;

namespace OnkormanyzatiPortal.Controllers;

[Authorize(Roles = "Vegrehajto")]
public class MunkafeladatokController(ApplicationDbContext db) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var feladatok = await SajatFeladatok()
            .AsNoTracking()
            .Include(f => f.Bejelentes)
            .OrderBy(f => f.Elkeszult)
            .ThenBy(f => f.Hatarido)
            .ToListAsync();

        return View(feladatok);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Rogzit(int id, string? megjegyzes, bool kesz)
    {
        var feladat = await SajatFeladatok()
            .Include(f => f.Bejelentes)
            .SingleOrDefaultAsync(f => f.Id == id);

        if (feladat is null)
            return NotFound();

        var ugy = feladat.Bejelentes;

        if (feladat.Elkeszult || ugy.Allapot != BejelentesAllapot.Folyamatban)
            return BadRequest("Ez a feladat már nem módosítható.");

        megjegyzes = megjegyzes?.Trim();

        if (!ModelState.IsValid || string.IsNullOrEmpty(megjegyzes) || megjegyzes.Length > 1000)
        {
            TempData["Hiba"] = "Adj meg helyszíni megjegyzést, legfeljebb 1000 karakterben.";
            return RedirectToAction(nameof(Index));
        }

        var elozoAllapot = ugy.Allapot;
        feladat.Megjegyzes = megjegyzes;

        if (kesz)
        {
            feladat.Elkeszult = true;

            var vanMasikNyitottFeladat = await db.Reszfeladatok.AnyAsync(f =>
                f.BejelentesId == ugy.Id && f.Id != id && !f.Elkeszult);

            if (!vanMasikNyitottFeladat)
                ugy.Allapot = BejelentesAllapot.LezarasraVar;
        }

        ugy.Verzio = Guid.NewGuid();

        db.BejelentesNaplok.Add(new BejelentesNaplo
        {
            BejelentesId = ugy.Id,
            Muvelet = kesz ? $"Részfeladat elkészült: #{id}" : $"Megjegyzés mentve: #{id}",
            ElozoAllapot = elozoAllapot,
            UjAllapot = ugy.Allapot,
            Hatarido = ugy.Hatarido,
            FelhasznaloId = User.FindFirstValue(ClaimTypes.NameIdentifier)
        });

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            TempData["Hiba"] = "Az ügy közben megváltozott. Próbáld újra a mentést.";
            return RedirectToAction(nameof(Index));
        }

        TempData["Siker"] = kesz ? "A részfeladat elkészült." : "A megjegyzést mentettük.";
        return RedirectToAction(nameof(Index));
    }

    private IQueryable<Reszfeladat> SajatFeladatok()
    {
        var felhasznaloId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        return db.Reszfeladatok.Where(f => db.SzervezetiTagok.Any(t =>
            t.FelhasznaloId == felhasznaloId && t.SzervezetId == f.SzervezetId));
    }
}