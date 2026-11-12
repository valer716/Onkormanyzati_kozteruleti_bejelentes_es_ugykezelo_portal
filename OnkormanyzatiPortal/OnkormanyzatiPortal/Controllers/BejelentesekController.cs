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
        [Bind("Cim,Leiras,Helyszin")] Bejelentes bejelentes)
    {
        if (!ModelState.IsValid)
        {
            return View(bejelentes);
        }

        bejelentes.LetrehozvaUtc = DateTime.UtcNow;
        _db.Bejelentesek.Add(bejelentes);
        await _db.SaveChangesAsync();

        TempData["Siker"] = $"A bejelentést mentettük. Azonosító: {bejelentes.Id}.";
        return RedirectToAction(nameof(Letrehozas));
    }

    [HttpGet]
    [Authorize(Roles = "Ugyintezo")]
    public async Task<IActionResult> Index()
    {
        var bejelentesek = await _db.Bejelentesek
            .AsNoTracking()
            .OrderByDescending(b => b.LetrehozvaUtc)
            .ToListAsync();

        return View(bejelentesek);
    }
}