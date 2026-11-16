using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnkormanyzatiPortal.Data;
using OnkormanyzatiPortal.Models;

namespace OnkormanyzatiPortal.Controllers;

[Authorize(Roles = "Vezeto")]
public class VezetoiController(ApplicationDbContext db) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var adatok = await db.Bejelentesek.AsNoTracking()
            .Select(b => new
            {
                b.Id,
                b.Cim,
                b.Helyszin,
                b.Kategoria,
                b.Allapot,
                b.Hatarido,
                b.Szelesseg,
                b.Hosszusag,
                b.LetrehozvaUtc,
                b.LezarvaUtc
            })
            .ToListAsync();

        var ma = DateTime.Today;

        var nyitottAllapotok = new[]
        {
            BejelentesAllapot.Uj,
            BejelentesAllapot.Ellenorizve,
            BejelentesAllapot.Folyamatban,
            BejelentesAllapot.LezarasraVar
        };

        var nyitott = adatok
            .Where(b => nyitottAllapotok.Contains(b.Allapot))
            .Select(b => new VezetoiUgySor(
                b.Id, b.Cim, b.Helyszin, b.Kategoria, b.Allapot, b.LetrehozvaUtc, b.Hatarido,
                b.Hatarido.HasValue && b.Hatarido.Value.Date < ma,
                (int)(DateTime.UtcNow - b.LetrehozvaUtc).TotalDays,
                b.Szelesseg, b.Hosszusag))
            .OrderByDescending(u => u.Lejart)
            .ThenBy(u => u.Hatarido ?? DateTime.MaxValue)
            .ThenBy(u => u.LetrehozvaUtc)
            .ToList();

        var atlag = adatok
            .Where(b => b.Allapot == BejelentesAllapot.Lezart
                && b.LezarvaUtc.HasValue)
            .Select(b => (double?)(b.LezarvaUtc!.Value - b.LetrehozvaUtc).TotalDays)
            .Average();

        var kategoriak = adatok
            .GroupBy(b => b.Kategoria)
            .OrderByDescending(g => g.Count())
            .ToDictionary(g => g.Key, g => g.Count());

        var allapotok = adatok
            .GroupBy(b => b.Allapot)
            .OrderBy(g => g.Key)
            .ToDictionary(g => g.Key, g => g.Count());

        var tizenketHonapja = new DateTime(ma.Year, ma.Month, 1).AddMonths(-11);

        var havi = Enumerable.Range(0, 12)
            .Select(i => tizenketHonapja.AddMonths(i))
            .Select(honap => new VezetoiHaviSor(
                honap.ToString("yyyy.MM"),
                adatok.Count(b => b.LetrehozvaUtc.Year == honap.Year
                    && b.LetrehozvaUtc.Month == honap.Month)))
            .ToList();

        var teruletek = adatok
            .GroupBy(b => Iranyitoszam(b.Helyszin))
            .Select(g => new VezetoiTerulet(
                g.Key,
                g.Count(),
                g.Count(b => nyitottAllapotok.Contains(b.Allapot)),
                g.Count(b => nyitottAllapotok.Contains(b.Allapot)
                    && b.Hatarido.HasValue && b.Hatarido.Value.Date < ma)))
            .OrderByDescending(t => t.Osszes)
            .ToList();

        return View(new VezetoiKimutatasNezet(
            nyitott,
            atlag,
            kategoriak,
            allapotok,
            havi,
            teruletek,
            adatok.Count,
            adatok.Count(b => b.Allapot == BejelentesAllapot.Lezart)));
    }

    [HttpGet]
    public async Task<IActionResult> Reszletek(int id)
    {
        var bejelentes = await db.Bejelentesek
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id);

        if (bejelentes is null)
            return NotFound();

        ViewBag.Reszfeladatok = await db.Reszfeladatok
            .AsNoTracking()
            .Include(f => f.Szervezet)
            .Where(f => f.BejelentesId == id)
            .OrderBy(f => f.Id)
            .ToListAsync();

        ViewBag.Fotok = await db.BejelentesFotok
            .AsNoTracking()
            .Where(f => f.BejelentesId == id)
            .OrderBy(f => f.Id)
            .Select(f => new FotoListaSor(f.Id, f.Tipus, f.ReszfeladatId))
            .ToListAsync();

        ViewBag.Naplo = await db.BejelentesNaplok
            .AsNoTracking()
            .Where(n => n.BejelentesId == id)
            .OrderBy(n => n.IdopontUtc)
            .ThenBy(n => n.Id)
            .ToListAsync();

        return View(bejelentes);
    }

    [HttpGet]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> Foto(int id)
    {
        var foto = await db.BejelentesFotok.AsNoTracking()
            .SingleOrDefaultAsync(f => f.Id == id);

        if (foto is null)
            return NotFound();

        Response.Headers["X-Content-Type-Options"] = "nosniff";
        return File(foto.Tartalom, foto.TartalomTipus);
    }

    [HttpGet]
    public async Task<IActionResult> Export()
    {
        var csoportok = await db.Bejelentesek
            .GroupBy(b => new { b.Kategoria, b.Allapot })
            .Select(g => new { g.Key.Kategoria, g.Key.Allapot, Darab = g.Count() })
            .ToListAsync();

        var adat = new
        {
            verzio = 1,
            keszultUtc = DateTime.UtcNow,
            sorok = csoportok.Select(c => new
            {
                kategoria = c.Kategoria.ToString(),
                allapot = c.Allapot.ToString(),
                darab = c.Darab
            })
        };

        return File(
            JsonSerializer.SerializeToUtf8Bytes(adat, new JsonSerializerOptions
            {
                WriteIndented = true
            }),
            "application/json",
            "bejelentesek-osszesites.json");
    }

    private static string Iranyitoszam(string helyszin)
    {
        var talalat = Regex.Match(helyszin, @"\b\d{4}\b");
        return talalat.Success ? talalat.Value : "Ismeretlen";
    }
}
