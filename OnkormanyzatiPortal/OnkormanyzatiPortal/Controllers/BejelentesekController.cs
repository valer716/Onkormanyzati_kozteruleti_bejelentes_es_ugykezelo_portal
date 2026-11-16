using Microsoft.AspNetCore.Mvc;
using OnkormanyzatiPortal.Data;
using OnkormanyzatiPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
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
    [RequestSizeLimit(6 * 1024 * 1024)]
    public async Task<IActionResult> Letrehozas(
    [Bind("Cim,Leiras,Helyszin,Kategoria")] Bejelentes bejelentes, string? szelesseg, string? hosszusag, IFormFile? foto)
    {
        double lat = 0;
        double lon = 0;

        if (string.IsNullOrWhiteSpace(szelesseg) || string.IsNullOrWhiteSpace(hosszusag))
        {
            ModelState.AddModelError("szelesseg", "Jelöld meg a hiba helyét a térképen, vagy add meg a koordinátákat.");
        }
        else
        {
            if (!KoordinataErvenyes(szelesseg, 90, out lat))
                ModelState.AddModelError("szelesseg", "A szélesség −90 és 90 közé essen.");

            if (!KoordinataErvenyes(hosszusag, 180, out lon))
                ModelState.AddModelError("hosszusag", "A hosszúság −180 és 180 közé essen.");
        }

        var koordinataHiba = ModelState["szelesseg"]?.Errors.Count > 0
            || ModelState["hosszusag"]?.Errors.Count > 0;

        if (string.IsNullOrWhiteSpace(bejelentes.Helyszin) && !koordinataHiba)
        {
            bejelentes.Helyszin = FormattableString.Invariant($"Térképi pont: {lat:0.00000}, {lon:0.00000}");
            ModelState.Remove(nameof(Bejelentes.Helyszin));
        }

        if (!ModelState.IsValid)
        {
            return View(bejelentes);
        }

        BejelentesFoto? kep;
        try
        {
            kep = await FotoFeltoltes.BeolvasAsync(foto, BejelentesFotoTipus.Bejelentes);
        }
        catch (ArgumentException e)
        {
            ModelState.AddModelError("foto", e.Message);
            return View(bejelentes);
        }

        bejelentes.LetrehozvaUtc = DateTime.UtcNow;
        bejelentes.Allapot = BejelentesAllapot.Uj;

        var kod = await UjKovetokodAsync();
        bejelentes.Kovetokod = kod;
        bejelentes.Szelesseg = lat;
        bejelentes.Hosszusag = lon;

        _db.Bejelentesek.Add(bejelentes);

        if (kep is not null)
        {
            kep.Bejelentes = bejelentes;
            _db.BejelentesFotok.Add(kep);
        }

        _db.BejelentesNaplok.Add(new BejelentesNaplo
        {
            Bejelentes = bejelentes,
            Muvelet = "Bejelentés érkezett",
            UjAllapot = BejelentesAllapot.Uj,
            FelhasznaloId = User.FindFirstValue(ClaimTypes.NameIdentifier)
        });
        await _db.SaveChangesAsync();

        TempData["Kovetokod"] = kod;
        return RedirectToAction(nameof(Kovetes));
    }
    private async Task<string> UjKovetokodAsync()
    {
        while (true)
        {
            var kod = RandomNumberGenerator.GetInt32(100000, 1000000).ToString(CultureInfo.InvariantCulture);

            if (!await _db.Bejelentesek.AnyAsync(b => b.Kovetokod == kod))
                return kod;
        }
    }

    private static string? KovetokodNormalizalas(string? bemenet)
    {
        var szamok = new string((bemenet ?? string.Empty).Where(char.IsDigit).ToArray());
        return szamok.Length == 6 ? szamok : null;
    }

    private static Expression<Func<Bejelentes, int>> KategoriaSorrend()
    {
        var kulturak = new CultureInfo("hu-HU");

        var sorrend = Enum.GetValues<BejelentesKategoria>()
            .OrderBy(k => k.GetType().GetField(k.ToString())!.GetCustomAttribute<DisplayAttribute>()!.Name, StringComparer.Create(kulturak, true))
            .ToList();

        var bejelentes = Expression.Parameter(typeof(Bejelentes), "b");
        Expression kifejezes = Expression.Constant(0);

        for (var i = sorrend.Count - 1; i >= 0; i--)
        {
            kifejezes = Expression.Condition(
                Expression.Equal(Expression.Property(bejelentes, nameof(Bejelentes.Kategoria)), Expression.Constant(sorrend[i])),
                Expression.Constant(i),
                kifejezes);
        }

        return Expression.Lambda<Func<Bejelentes, int>>(kifejezes, bejelentes);
    }

    private static bool KoordinataErvenyes(string? szoveg, double hatar, out double ertek)
    {
        return double.TryParse(
            szoveg?.Trim().Replace(',', '.'),
            System.Globalization.NumberStyles.Float,
            System.Globalization.CultureInfo.InvariantCulture,
            out ertek)
            && double.IsFinite(ertek)
            && Math.Abs(ertek) <= hatar;
    }

    [HttpGet]
    [Authorize(Roles = "Ugyintezo")]
    public async Task<IActionResult> Index(
        string? kereses,
        BejelentesKategoria[]? kategoria,
        BejelentesAllapot[]? allapot,
        string? rendez,
        string? irany,
        int oldal = 1)
    {
        const int oldalMeret = 50;
        var lekerdezes = _db.Bejelentesek.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(kereses))
        {
            var szo = kereses.Trim();
            lekerdezes = lekerdezes.Where(b =>
                b.Cim.Contains(szo) || b.Helyszin.Contains(szo));
        }

        if (kategoria is { Length: > 0 })
            lekerdezes = lekerdezes.Where(b => kategoria.Contains(b.Kategoria));

        if (allapot is { Length: > 0 })
            lekerdezes = lekerdezes.Where(b => allapot.Contains(b.Allapot));

        var csokkeno = irany == "csokkeno";

        var darab = await lekerdezes.CountAsync();
        var oldalSzam = Math.Max(1, (int)Math.Ceiling(darab / (double)oldalMeret));
        oldal = Math.Clamp(oldal, 1, oldalSzam);

        var rendezett = rendez switch
        {
            "azonosito" => csokkeno ? lekerdezes.OrderByDescending(b => b.Id) : lekerdezes.OrderBy(b => b.Id),
            "cim" => csokkeno ? lekerdezes.OrderByDescending(b => b.Cim) : lekerdezes.OrderBy(b => b.Cim),
            "helyszin" => csokkeno ? lekerdezes.OrderByDescending(b => b.Helyszin) : lekerdezes.OrderBy(b => b.Helyszin),
            "kategoria" => csokkeno
                ? lekerdezes.OrderByDescending(KategoriaSorrend()).ThenBy(b => b.Id)
                : lekerdezes.OrderBy(KategoriaSorrend()).ThenBy(b => b.Id),
            "allapot" => csokkeno ? lekerdezes.OrderByDescending(b => b.Allapot) : lekerdezes.OrderBy(b => b.Allapot),
            "beerkezes" => csokkeno ? lekerdezes.OrderByDescending(b => b.LetrehozvaUtc) : lekerdezes.OrderBy(b => b.LetrehozvaUtc),
            _ => lekerdezes.OrderByDescending(b => b.Veszelyes).ThenByDescending(b => b.LetrehozvaUtc)
        };

        var lista = await rendezett
            .Skip((oldal - 1) * oldalMeret)
            .Take(oldalMeret)
            .ToListAsync();

        var azonositok = lista.Select(b => b.Id).ToList();

        ViewBag.Kepek = await _db.BejelentesFotok
            .AsNoTracking()
            .Where(f => f.Tipus == BejelentesFotoTipus.Bejelentes && azonositok.Contains(f.BejelentesId))
            .GroupBy(f => f.BejelentesId)
            .Select(g => new { BejelentesId = g.Key, FotoId = g.Min(f => f.Id) })
            .ToDictionaryAsync(g => g.BejelentesId, g => g.FotoId);

        ViewBag.Oldal = oldal;
        ViewBag.OldalSzam = oldalSzam;
        ViewBag.Darab = darab;

        return View(lista);
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

        return await ReszletekNezet(bejelentes);
    }

    [HttpPost]
    [Authorize(Roles = "Ugyintezo")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Ellenoriz(int id, DateTime? hatarido)
    {
        var bejelentes = await _db.Bejelentesek.FindAsync(id);

        if (bejelentes is null)
            return NotFound();

        if (bejelentes.Allapot is not
            (BejelentesAllapot.Uj or BejelentesAllapot.Ellenorizve))
        {
            return BadRequest("Ebben az állapotban az ügy már nem ellenőrizhető.");
        }

        if (hatarido is null || hatarido.Value.Date < DateTime.Today)
        {
            TempData["Hiba"] = "A határidő ma vagy később legyen.";
            return RedirectToAction(nameof(Reszletek), "Bejelentesek", new { id }, "kovetkezo");
        }

        var elozoAllapot = bejelentes.Allapot;

        bejelentes.Hatarido = hatarido.Value.Date;
        bejelentes.Allapot = BejelentesAllapot.Ellenorizve;

        _db.BejelentesNaplok.Add(new BejelentesNaplo
        {
            BejelentesId = bejelentes.Id,
            Muvelet = elozoAllapot == BejelentesAllapot.Uj
                ? "Ellenőrzés és határidő beállítása"
                : "Határidő módosítása",
            ElozoAllapot = elozoAllapot,
            UjAllapot = bejelentes.Allapot,
            Hatarido = bejelentes.Hatarido,
            FelhasznaloId = User.FindFirstValue(ClaimTypes.NameIdentifier)
        });

        bejelentes.Verzio = Guid.NewGuid();

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            TempData["Hiba"] = "Az ügy közben megváltozott. Nézd át, majd próbáld újra.";
            return RedirectToAction(nameof(Reszletek), new { id });
        }

        TempData["Siker"] = "Az ügyet ellenőrzöttnek jelöltük.";
        return RedirectToAction(nameof(Reszletek), new { id });
    }

    [HttpPost]
    [Authorize(Roles = "Ugyintezo")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Pontosit(
        int id, string? cim, string? leiras, string? helyszin, BejelentesKategoria kategoria)
    {
        var bejelentes = await _db.Bejelentesek.FindAsync(id);

        if (bejelentes is null)
            return NotFound();

        if (bejelentes.Allapot is not (BejelentesAllapot.Uj or BejelentesAllapot.Ellenorizve
            or BejelentesAllapot.Folyamatban or BejelentesAllapot.LezarasraVar))
        {
            return BadRequest("Ebben az állapotban az ügy adatai már nem módosíthatók.");
        }

        cim = cim?.Trim();
        leiras = leiras?.Trim();
        helyszin = helyszin?.Trim();

        if (string.IsNullOrEmpty(cim) || cim.Length > 120
            || string.IsNullOrEmpty(leiras) || leiras.Length > 2000
            || string.IsNullOrEmpty(helyszin) || helyszin.Length > 200
            || !Enum.IsDefined(kategoria))
        {
            TempData["Hiba"] = "Adj meg címet (legfeljebb 120), leírást (legfeljebb 2000) és helyszínt (legfeljebb 200 karakter).";
            return RedirectToAction(nameof(Reszletek), "Bejelentesek", new { id }, "adatok");
        }

        bejelentes.Cim = cim;
        bejelentes.Leiras = leiras;
        bejelentes.Helyszin = helyszin;
        bejelentes.Kategoria = kategoria;
        bejelentes.Verzio = Guid.NewGuid();

        _db.BejelentesNaplok.Add(new BejelentesNaplo
        {
            BejelentesId = bejelentes.Id,
            Muvelet = "Ügyadatok pontosítva",
            ElozoAllapot = bejelentes.Allapot,
            UjAllapot = bejelentes.Allapot,
            Hatarido = bejelentes.Hatarido,
            FelhasznaloId = User.FindFirstValue(ClaimTypes.NameIdentifier)
        });

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            TempData["Hiba"] = "Az ügy közben megváltozott. Nézd át, majd próbáld újra.";
            return RedirectToAction(nameof(Reszletek), "Bejelentesek", new { id }, "adatok");
        }

        TempData["Siker"] = "Az ügy adatait mentettük.";
        return RedirectToAction(nameof(Reszletek), "Bejelentesek", new { id }, "adatok");
    }

    private async Task<List<FoUgyJelolt>> FoUgyJeloltek(Bejelentes bejelentes)
    {
        var jeloltek = await _db.Bejelentesek
            .AsNoTracking()
            .Where(b => b.Id != bejelentes.Id
                && b.Kategoria == bejelentes.Kategoria
                && (b.Allapot == BejelentesAllapot.Ellenorizve
                    || b.Allapot == BejelentesAllapot.Folyamatban
                    || b.Allapot == BejelentesAllapot.LezarasraVar))
            .OrderByDescending(b => b.LetrehozvaUtc)
            .Take(200)
            .Select(b => new FoUgyJelolt(b.Id, b.Cim, b.Helyszin, b.Szelesseg, b.Hosszusag, null))
            .ToListAsync();

        if (bejelentes.Szelesseg is double lat && bejelentes.Hosszusag is double lon)
        {
            jeloltek = jeloltek
                .Select(j => j with { TavolsagMeter = j.Szelesseg is double jLat && j.Hosszusag is double jLon
                    ? Tavolsag(lat, lon, jLat, jLon)
                    : null })
                .OrderBy(j => j.TavolsagMeter ?? double.MaxValue)
                .ToList();
        }

        return jeloltek.Take(15).ToList();
    }

    private static double Tavolsag(double lat1, double lon1, double lat2, double lon2)
    {
        const double fold = 6371000;
        var fok = Math.PI / 180;
        var dLat = (lat2 - lat1) * fok;
        var dLon = (lon2 - lon1) * fok;
        var a = Math.Pow(Math.Sin(dLat / 2), 2)
            + Math.Cos(lat1 * fok) * Math.Cos(lat2 * fok) * Math.Pow(Math.Sin(dLon / 2), 2);

        return fold * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }

    private async Task<IActionResult> ReszletekNezet(Bejelentes bejelentes)
    {
        ViewBag.Reszfeladatok = await _db.Reszfeladatok
            .AsNoTracking()
            .Include(f => f.Szervezet)
            .Where(f => f.BejelentesId == bejelentes.Id)
            .OrderBy(f => f.Id)
            .ToListAsync();

        ViewBag.Szervezetek = await _db.Szervezetek
            .AsNoTracking()
            .OrderBy(s => s.Nev)
            .ToListAsync();

        ViewBag.Fotok = await _db.BejelentesFotok
            .Where(f => f.BejelentesId == bejelentes.Id)
            .OrderBy(f => f.Id)
            .Select(f => new FotoListaSor(f.Id, f.Tipus, f.ReszfeladatId))
            .ToListAsync();

        ViewBag.Naplo = await _db.BejelentesNaplok
            .AsNoTracking()
            .Where(n => n.BejelentesId == bejelentes.Id)
            .OrderBy(n => n.IdopontUtc)
            .ThenBy(n => n.Id)
            .ToListAsync();

        ViewBag.Felhasznalok = await _db.Users
            .AsNoTracking()
            .ToDictionaryAsync(u => u.Id, u => u.UserName ?? string.Empty);

        ViewBag.FoUgyek = await FoUgyJeloltek(bejelentes);

        ViewBag.JelzesekSzama = 1 + await _db.Bejelentesek
            .CountAsync(b => b.FoBejelentesId == bejelentes.Id);

        return View(nameof(Reszletek), bejelentes);
    }

    [HttpPost]
    [Authorize(Roles = "Ugyintezo")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Kioszt(
    int id, int szervezetId, string? feladatCim, DateTime? feladatHatarido)
    {
        var bejelentes = await _db.Bejelentesek.FindAsync(id);

        if (bejelentes is null)
            return NotFound();

        if (bejelentes.Allapot is not
            (BejelentesAllapot.Ellenorizve or BejelentesAllapot.Folyamatban)
            || bejelentes.Hatarido is null)
        {
            return BadRequest("Csak ellenőrzött, határidővel rendelkező ügyhöz osztható ki feladat.");
        }

        feladatCim = feladatCim?.Trim();

        if (!ModelState.IsValid
            || string.IsNullOrEmpty(feladatCim)
            || feladatCim.Length > 160
            || feladatHatarido is null
            || feladatHatarido.Value.Date < DateTime.Today
            || feladatHatarido.Value.Date > bejelentes.Hatarido.Value.Date
            || await _db.Szervezetek.FindAsync(szervezetId) is null)
        {
            TempData["Hiba"] = "Adj meg feladatcímet, szervezetet és érvényes határidőt. "
                + "A feladat határideje nem lehet múltbeli vagy későbbi az ügy határidejénél.";

            return RedirectToAction(nameof(Reszletek), new { id });
        }

        var elozoAllapot = bejelentes.Allapot;
        var hatarido = feladatHatarido.Value.Date;

        _db.Reszfeladatok.Add(new Reszfeladat
        {
            BejelentesId = id,
            SzervezetId = szervezetId,
            Cim = feladatCim,
            Hatarido = hatarido
        });

        bejelentes.Allapot = BejelentesAllapot.Folyamatban;

        _db.BejelentesNaplok.Add(new BejelentesNaplo
        {
            BejelentesId = id,
            Muvelet = $"Feladat kiosztva: {(await _db.Szervezetek.FindAsync(szervezetId))!.Nev}, határidő {hatarido:yyyy-MM-dd}",
            ElozoAllapot = elozoAllapot,
            UjAllapot = bejelentes.Allapot,
            Hatarido = bejelentes.Hatarido,
            FelhasznaloId = User.FindFirstValue(ClaimTypes.NameIdentifier)
        });

        bejelentes.Verzio = Guid.NewGuid();

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            TempData["Hiba"] = "Az ügy közben megváltozott. Nézd át, majd próbáld újra.";
            return RedirectToAction(nameof(Reszletek), new { id });
        }
        TempData["Siker"] = "A részfeladatot kiosztottuk.";
        return RedirectToAction(nameof(Reszletek), new { id });
    }
    [HttpPost]
    [Authorize(Roles = "Ugyintezo")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Lezar(int id)
    {
        var bejelentes = await _db.Bejelentesek.FindAsync(id);

        if (bejelentes is null)
            return NotFound();

        var teljesitesek = await _db.Reszfeladatok
            .Where(f => f.BejelentesId == id)
            .Select(f => f.Elkeszult)
            .ToListAsync();

        if (bejelentes.Allapot != BejelentesAllapot.LezarasraVar
            || teljesitesek.Count == 0
            || teljesitesek.Any(elkeszult => !elkeszult))
        {
            TempData["Hiba"] = "Az ügy csak akkor zárható le, ha minden részfeladat elkészült.";
            return RedirectToAction(nameof(Reszletek), new { id });
        }

        var lezarasIdeje = DateTime.UtcNow;
        bejelentes.Allapot = BejelentesAllapot.Lezart;
        bejelentes.LezarvaUtc = lezarasIdeje;
        bejelentes.Verzio = Guid.NewGuid();

        _db.BejelentesNaplok.Add(new BejelentesNaplo
        {
            BejelentesId = id,
            Muvelet = "Ügyintéző ellenőrizte és lezárta az ügyet",
            ElozoAllapot = BejelentesAllapot.LezarasraVar,
            UjAllapot = BejelentesAllapot.Lezart,
            Hatarido = bejelentes.Hatarido,
            IdopontUtc = lezarasIdeje,
            FelhasznaloId = User.FindFirstValue(ClaimTypes.NameIdentifier)
        });

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            TempData["Hiba"] = "Az ügy közben megváltozott. Nézd át újra a részleteket.";
            return RedirectToAction(nameof(Reszletek), new { id });
        }

        TempData["Siker"] = "Az ügyet lezártuk.";
        return RedirectToAction(nameof(Reszletek), new { id });
    }
    [HttpGet]
    [AllowAnonymous]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> Kovetes(string? kovetokod)
    {
        if (string.IsNullOrWhiteSpace(kovetokod))
            return View();

        return await KovetesKeres(kovetokod);
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    [ActionName("Kovetes")]
    public async Task<IActionResult> KovetesKeres(string? kovetokod)
    {
        var kod = KovetokodNormalizalas(kovetokod);

        if (!ModelState.IsValid || kod is null)
        {
            ModelState.AddModelError("", "Adj meg érvényes, 6 számjegyű követőkódot.");
            return View();
        }

        var eredmeny = await _db.Bejelentesek
            .AsNoTracking()
            .Where(b => b.Kovetokod == kod)
            .Select(b => new BejelentesKovetesNezet(
                b.Id,
                kod,
                b.FoBejelentes == null ? b.Allapot : b.FoBejelentes.Allapot,
                b.FoBejelentes == null ? b.Hatarido : b.FoBejelentes.Hatarido,
                b.Ertekeles))
            .SingleOrDefaultAsync();

        if (eredmeny is null)
        {
            ModelState.AddModelError("", "Nem található bejelentés ezzel a kóddal.");
            return View(eredmeny);
        }

        var esemenyek = await _db.BejelentesNaplok
            .AsNoTracking()
            .Where(n => n.BejelentesId == eredmeny.Id && n.ElozoAllapot != n.UjAllapot)
            .OrderBy(n => n.IdopontUtc)
            .ThenBy(n => n.Id)
            .Select(n => new KovetesEsemeny(n.IdopontUtc, n.UjAllapot))
            .ToListAsync();

        return View(eredmeny with { Esemenyek = esemenyek });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> Ertekel(string? kovetokod, int pont)
    {
        var kod = KovetokodNormalizalas(kovetokod);

        if (!ModelState.IsValid || kod is null || pont is < 1 or > 5)
        {
            return BadRequest("Érvényes követőkód és 1–5 közötti pontszám szükséges.");
        }

        var bejelentes = await _db.Bejelentesek.Include(b => b.FoBejelentes)
            .SingleOrDefaultAsync(b => b.Kovetokod == kod);

        if (bejelentes is null)
            return NotFound();

        var kovetettUgy = bejelentes.FoBejelentes ?? bejelentes;

        if (kovetettUgy.Allapot != BejelentesAllapot.Lezart
            || bejelentes.Ertekeles.HasValue)
        {
            return BadRequest("Csak lezárt ügy értékelhető, és csak egyszer.");
        }

        bejelentes.Ertekeles = pont;
        bejelentes.Verzio = Guid.NewGuid();

        _db.BejelentesNaplok.Add(new BejelentesNaplo
        {
            BejelentesId = bejelentes.Id,
            Muvelet = $"Lakossági értékelés: {pont}/5",
            ElozoAllapot = bejelentes.Allapot,
            UjAllapot = bejelentes.Allapot,
            Hatarido = bejelentes.Hatarido,
            FelhasznaloId = User.FindFirstValue(ClaimTypes.NameIdentifier)
        });

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return Conflict("Az ügy közben megváltozott. Kérd le újra az állapotát.");
        }

        return View(nameof(Kovetes), new BejelentesKovetesNezet(
            bejelentes.Id, kod, kovetettUgy.Allapot,
            kovetettUgy.Hatarido, bejelentes.Ertekeles));
    }

    [HttpGet]
    [Authorize(Roles = "Ugyintezo")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> Foto(int id)
    {
        var foto = await _db.BejelentesFotok.AsNoTracking()
            .SingleOrDefaultAsync(f => f.Id == id);

        if (foto is null)
            return NotFound();

        Response.Headers["X-Content-Type-Options"] = "nosniff";
        return File(foto.Tartalom, foto.TartalomTipus);
    }
    [HttpPost]
    [Authorize(Roles = "Ugyintezo")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> KulonlegesKezeles(
    int id, string? muvelet, string? indok, int? foBejelentesId)
    {
        if (!ModelState.IsValid)
            return Hiba("Ellenőrizd a megadott adatokat.");

        var ugy = await _db.Bejelentesek.FindAsync(id);
        if (ugy is null)
            return NotFound();

        var elozo = ugy.Allapot;
        var kiosztasElott = elozo is BejelentesAllapot.Uj
            or BejelentesAllapot.Ellenorizve;

        if (!kiosztasElott &&
            elozo is not (BejelentesAllapot.Folyamatban or BejelentesAllapot.LezarasraVar))
        {
            return Hiba("Végleges állapotú ügy már nem módosítható.");
        }

        string naploSzoveg;

        if (muvelet is "veszelyes" or "normal")
        {
            ugy.Veszelyes = muvelet == "veszelyes";
            naploSzoveg = ugy.Veszelyes
                ? "Veszélyesnek jelölve"
                : "Veszélyjelzés levéve";
        }
        else
        {
            if (!kiosztasElott ||
                await _db.Reszfeladatok.AnyAsync(f => f.BejelentesId == id))
            {
                return Hiba("Ez a döntés csak a feladatok kiosztása előtt rögzíthető.");
            }

            if (muvelet == "osszevon")
            {
                if (foBejelentesId is not int foId || foId == id)
                    return Hiba("Adj meg egy másik ügyazonosítót.");

                var fo = await _db.Bejelentesek.FindAsync(foId);
                if (fo is null || fo.Kategoria != ugy.Kategoria ||
                    fo.Allapot is not (BejelentesAllapot.Ellenorizve
                        or BejelentesAllapot.Folyamatban or BejelentesAllapot.LezarasraVar))
                {
                    return Hiba("A főügy legyen azonos kategóriájú, ellenőrzött és még nyitott.");
                }

                if (await _db.Bejelentesek.AnyAsync(b => b.FoBejelentesId == id))
                    return Hiba("Ehhez az ügyhöz már tartoznak összevont jelzések.");

                ugy.FoBejelentesId = fo.Id;
                ugy.Allapot = BejelentesAllapot.Osszevont;
                fo.Veszelyes |= ugy.Veszelyes;

                Naploz(fo, fo.Allapot, $"Kapcsolódó jelzés hozzáadva: #{id}");
                naploSzoveg = $"Összevonva a(z) #{fo.Id} üggyel";
            }
            else if (muvelet is "teves" or "hataskor")
            {
                indok = indok?.Trim();
                if (string.IsNullOrEmpty(indok) || indok.Length > 80)
                    return Hiba("Adj meg legfeljebb 80 karakteres indoklást.");

                ugy.Allapot = muvelet == "teves"
                    ? BejelentesAllapot.Teves
                    : BejelentesAllapot.HataskoronKivuli;

                naploSzoveg = (muvelet == "teves" ? "Téves: " : "Hatáskörön kívüli: ") + indok;
            }
            else
            {
                return Hiba("Ismeretlen művelet.");
            }
        }

        Naploz(ugy, elozo, naploSzoveg);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return Hiba("Az ügy közben megváltozott. Nézd át, majd próbáld újra.");
        }

        TempData["Siker"] = "A módosítást mentettük.";
        return RedirectToAction(nameof(Reszletek), "Bejelentesek", new { id }, "kulonleges");

        IActionResult Hiba(string uzenet)
        {
            TempData["Hiba"] = uzenet;
            return RedirectToAction(nameof(Reszletek), "Bejelentesek", new { id }, "kulonleges");
        }

        void Naploz(Bejelentes bejelentes, BejelentesAllapot elozoAllapot, string szoveg)
        {
            bejelentes.Verzio = Guid.NewGuid();

            _db.BejelentesNaplok.Add(new BejelentesNaplo
            {
                BejelentesId = bejelentes.Id,
                Muvelet = szoveg,
                ElozoAllapot = elozoAllapot,
                UjAllapot = bejelentes.Allapot,
                Hatarido = bejelentes.Hatarido,
                FelhasznaloId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            });
        }
    }
}