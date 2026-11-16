using System.Text.Json;
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

        var nyitott = adatok
            .Where(b => b.Allapot is BejelentesAllapot.Uj
                or BejelentesAllapot.Ellenorizve
                or BejelentesAllapot.Folyamatban
                or BejelentesAllapot.LezarasraVar)
            .OrderBy(b => b.Hatarido ?? DateTime.MaxValue)
            .Select(b => new VezetoiUgySor(
                b.Id, b.Kategoria, b.Allapot, b.Hatarido,
                b.Hatarido.HasValue && b.Hatarido.Value.Date < ma,
                b.Szelesseg, b.Hosszusag))
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

        return View(new VezetoiKimutatasNezet(nyitott, atlag, kategoriak));
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
}