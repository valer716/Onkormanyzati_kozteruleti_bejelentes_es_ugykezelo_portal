using Microsoft.AspNetCore.Mvc;
using OnkormanyzatiPortal.Data;
using OnkormanyzatiPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
namespace OnkormanyzatiPortal.Controllers;

public class BejelentesekController : Controller
{
    private readonly ApplicationDbContext _db;

    public BejelentesekController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public IActionResult Letrehozas()
    {
        return View(new Bejelentes());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Letrehozas(
        [Bind("Cim,Leiras,Helyszin,Kategoria")] Bejelentes bejelentes)
    {
        if (!ModelState.IsValid)
        {
            return View(bejelentes);
        }

        bejelentes.LetrehozvaUtc = DateTime.UtcNow;
        bejelentes.Allapot = BejelentesAllapot.Uj;
        _db.Bejelentesek.Add(bejelentes);
        await _db.SaveChangesAsync();

        TempData["Siker"] = $"A bejelentést mentettük. Azonosító: {bejelentes.Id}.";
        return RedirectToAction(nameof(Letrehozas));
    }

    [HttpGet]
    [Authorize(Roles = "Ugyintezo")]
    public async Task<IActionResult> Index(
        string? kereses,
        BejelentesKategoria? kategoria,
        BejelentesAllapot? allapot)
    {
        var lekerdezes = _db.Bejelentesek.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(kereses))
        {
            var szo = kereses.Trim();
            lekerdezes = lekerdezes.Where(b =>
                b.Cim.Contains(szo) || b.Helyszin.Contains(szo));
        }

        if (kategoria.HasValue)
            lekerdezes = lekerdezes.Where(b => b.Kategoria == kategoria.Value);

        if (allapot.HasValue)
            lekerdezes = lekerdezes.Where(b => b.Allapot == allapot.Value);

        return View(await lekerdezes
            .OrderByDescending(b => b.LetrehozvaUtc)
            .ToListAsync());
    }
    [HttpGet]
    [Authorize(Roles = "Ugyintezo")]
    public async Task<IActionResult> Reszletek(int id)
    {
        var bejelentes = await _db.Bejelentesek
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id);

        if (bejelentes is null)
            return NotFound();

        return View(bejelentes);
    }

    [HttpPost]
    [Authorize(Roles = "Ugyintezo")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Ellenoriz(int id)
    {
        var bejelentes = await _db.Bejelentesek.FindAsync(id);

        if (bejelentes is null)
            return NotFound();

        if (bejelentes.Allapot == BejelentesAllapot.Uj)
        {
            bejelentes.Allapot = BejelentesAllapot.Ellenorizve;
            await _db.SaveChangesAsync();
            TempData["Siker"] = "A bejelentés állapota: Ellenőrizve.";
        }

        return RedirectToAction(nameof(Reszletek), new { id });
    }
}