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
    public async Task<IActionResult> Index(string? kereses, string? allapot)
    {
        IQueryable<Reszfeladat> lekerdezes = SajatFeladatok()
            .AsNoTracking()
            .Include(f => f.Bejelentes);

        if (!string.IsNullOrWhiteSpace(kereses))
        {
            var szo = kereses.Trim();
            lekerdezes = lekerdezes.Where(f =>
                f.Cim.Contains(szo)
                || f.Bejelentes.Cim.Contains(szo)
                || f.Bejelentes.Helyszin.Contains(szo));
        }

        if (allapot == "nyitott")
            lekerdezes = lekerdezes.Where(f => !f.Elkeszult);
        else if (allapot == "kesz")
            lekerdezes = lekerdezes.Where(f => f.Elkeszult);

        var feladatok = await lekerdezes
            .OrderBy(f => f.Elkeszult)
            .ThenBy(f => f.Hatarido)
            .ToListAsync();

        var ugyAzonositok = feladatok.Select(f => f.BejelentesId).Distinct().ToList();

        ViewBag.Kepek = await db.BejelentesFotok
            .AsNoTracking()
            .Where(f => f.Tipus == BejelentesFotoTipus.Bejelentes && ugyAzonositok.Contains(f.BejelentesId))
            .GroupBy(f => f.BejelentesId)
            .Select(g => new { BejelentesId = g.Key, FotoId = g.Min(f => f.Id) })
            .ToDictionaryAsync(g => g.BejelentesId, g => g.FotoId);

        return View(feladatok);
    }

    [HttpGet]
    public async Task<IActionResult> Reszletek(int id)
    {
        var feladat = await SajatFeladatok()
            .AsNoTracking()
            .Include(f => f.Bejelentes)
            .Include(f => f.Szervezet)
            .SingleOrDefaultAsync(f => f.Id == id);

        if (feladat is null)
            return NotFound();

        ViewBag.Fotok = await db.BejelentesFotok
            .AsNoTracking()
            .Where(f => f.BejelentesId == feladat.BejelentesId
                && (f.ReszfeladatId == null || f.ReszfeladatId == id))
            .OrderBy(f => f.Id)
            .Select(f => new FotoListaSor(f.Id, f.Tipus, f.ReszfeladatId))
            .ToListAsync();

        return View(feladat);
    }

    [HttpGet]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> Foto(int id)
    {
        var felhasznaloId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var foto = await db.BejelentesFotok
            .AsNoTracking()
            .Where(f => f.Id == id
                && db.SzervezetiTagok.Any(t => t.FelhasznaloId == felhasznaloId
                    && db.Reszfeladatok.Any(x => x.SzervezetId == t.SzervezetId
                        && x.BejelentesId == f.BejelentesId
                        && (f.ReszfeladatId == null || f.ReszfeladatId == x.Id))))
            .SingleOrDefaultAsync();

        if (foto is null)
            return NotFound();

        Response.Headers["X-Content-Type-Options"] = "nosniff";
        return File(foto.Tartalom, foto.TartalomTipus);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(12 * 1024 * 1024)]
    public async Task<IActionResult> Rogzit(int id, string? megjegyzes, bool kesz, IFormFile? elotte, IFormFile? utana)
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
            return RedirectToAction(nameof(Reszletek), new { id });
        }

        BejelentesFoto? elotteKep;
        BejelentesFoto? utanaKep;

        try
        {
            elotteKep = await FotoFeltoltes.BeolvasAsync(elotte, BejelentesFotoTipus.Elotte);
            utanaKep = await FotoFeltoltes.BeolvasAsync(utana, BejelentesFotoTipus.Utana);
        }
        catch (ArgumentException e)
        {
            TempData["Hiba"] = e.Message;
            return RedirectToAction(nameof(Reszletek), new { id });
        }

        foreach (var kep in new[] { elotteKep, utanaKep })
        {
            if (kep is null)
                continue;

            var regi = await db.BejelentesFotok.SingleOrDefaultAsync(f =>
                f.ReszfeladatId == id && f.Tipus == kep.Tipus);

            if (regi is not null)
                db.BejelentesFotok.Remove(regi);

            kep.BejelentesId = ugy.Id;
            kep.ReszfeladatId = id;
            db.BejelentesFotok.Add(kep);
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
            Muvelet = $"{(kesz ? "Részfeladat elkészült" : "Megjegyzés mentve")}: #{id}; " + $"fotók: {(elotteKep is null ? 0 : 1) + (utanaKep is null ? 0 : 1)}",            ElozoAllapot = elozoAllapot,
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
            return RedirectToAction(nameof(Reszletek), new { id });
        }

        TempData["Siker"] = kesz ? "A részfeladat elkészült." : "A megjegyzést mentettük.";

        if (kesz)
            return RedirectToAction(nameof(Index));

        return RedirectToAction(nameof(Reszletek), new { id });
    }

    private IQueryable<Reszfeladat> SajatFeladatok()
    {
        var felhasznaloId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        return db.Reszfeladatok.Where(f => db.SzervezetiTagok.Any(t =>
            t.FelhasznaloId == felhasznaloId && t.SzervezetId == f.SzervezetId));
    }
}