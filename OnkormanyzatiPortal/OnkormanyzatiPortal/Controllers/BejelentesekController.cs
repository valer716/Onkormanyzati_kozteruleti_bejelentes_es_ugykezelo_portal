using Microsoft.AspNetCore.Mvc;
using OnkormanyzatiPortal.Data;
using OnkormanyzatiPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
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
        _db.BejelentesNaplok.Add(new BejelentesNaplo
        {
            Bejelentes = bejelentes,
            Muvelet = "Bejelentés érkezett",
            UjAllapot = BejelentesAllapot.Uj,
            FelhasznaloId = User.FindFirstValue(ClaimTypes.NameIdentifier)
        });
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
    public async Task<IActionResult> Ellenoriz(
        int id, string? cim, string? leiras, string? helyszin, DateTime? hatarido)
    {
        var bejelentes = await _db.Bejelentesek.FindAsync(id);

        if (bejelentes is null)
            return NotFound();

        if (bejelentes.Allapot is not
            (BejelentesAllapot.Uj or BejelentesAllapot.Ellenorizve))
        {
            return BadRequest("Ebben az állapotban az ügy már nem pontosítható.");
        }

        cim = cim?.Trim();
        leiras = leiras?.Trim();
        helyszin = helyszin?.Trim();

        if (string.IsNullOrEmpty(cim) || cim.Length > 120)
            ModelState.AddModelError("Cim", "Adj meg címet, legfeljebb 120 karakterben.");

        if (string.IsNullOrEmpty(leiras) || leiras.Length > 2000)
            ModelState.AddModelError("Leiras", "Adj meg leírást, legfeljebb 2000 karakterben.");

        if (string.IsNullOrEmpty(helyszin) || helyszin.Length > 200)
            ModelState.AddModelError("Helyszin", "Adj meg helyszínt, legfeljebb 200 karakterben.");

        if (hatarido is null || hatarido.Value.Date < DateTime.Today)
            ModelState.AddModelError("Hatarido", "A határidő ma vagy később legyen.");

        if (!ModelState.IsValid)
            return View(nameof(Reszletek), bejelentes);

        var elozoAllapot = bejelentes.Allapot;

        bejelentes.Cim = cim!;
        bejelentes.Leiras = leiras!;
        bejelentes.Helyszin = helyszin!;
        bejelentes.Hatarido = hatarido!.Value.Date;
        bejelentes.Allapot = BejelentesAllapot.Ellenorizve;

        _db.BejelentesNaplok.Add(new BejelentesNaplo
        {
            BejelentesId = bejelentes.Id,
            Muvelet = elozoAllapot == BejelentesAllapot.Uj
                ? "Ellenőrzés és határidő beállítása"
                : "Ügyadatok és határidő pontosítása",
            ElozoAllapot = elozoAllapot,
            UjAllapot = bejelentes.Allapot,
            Hatarido = bejelentes.Hatarido,
            FelhasznaloId = User.FindFirstValue(ClaimTypes.NameIdentifier)
        });

        await _db.SaveChangesAsync();

        TempData["Siker"] = "Az ellenőrzött ügy adatait mentettük.";
        return RedirectToAction(nameof(Reszletek), new { id });
    }
}