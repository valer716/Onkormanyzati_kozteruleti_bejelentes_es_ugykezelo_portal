using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnkormanyzatiPortal.Models;

namespace OnkormanyzatiPortal.Data;

public static class MintaAdatBetoltes
{
    public const string DemoJelszo = "Demo!2026";

    private static readonly DemoFiok[] DemoFiokok =
    [
        new("erika.ugyintezo@portal.test", "Ugyintezo", null),
        new("gabor.ugyintezo@portal.test", "Ugyintezo", null),
        new("nora.ugyintezo@portal.test", "Ugyintezo", null),
        new("bela.vezeto@portal.test", "Vezeto", null),
        new("balint.utfenntartas@portal.test", "Vegrehajto", 1),
        new("kata.koztisztasag@portal.test", "Vegrehajto", 2),
        new("peter.kozvilagitas@portal.test", "Vegrehajto", 3)
    ];

    private static readonly Dictionary<string, int> SzervezetAzonositok = new()
    {
        ["Utfenntartas"] = 1,
        ["Koztisztasag"] = 2,
        ["Kozvilagitas"] = 3
    };

    public static async Task Betolt(IServiceProvider szolgaltatok, string tartalomGyoker)
    {
        var felhasznalok = szolgaltatok.GetRequiredService<UserManager<IdentityUser>>();
        var db = szolgaltatok.GetRequiredService<ApplicationDbContext>();

        await FiokokLetrehozasa(felhasznalok, db);

        if (await db.Bejelentesek.AnyAsync())
            return;

        var mappa = Path.GetFullPath(Path.Combine(tartalomGyoker, "..", "..", "MintaAdatok"));
        var fajl = Path.Combine(mappa, "mintaadatok.json");
        if (!File.Exists(fajl))
            return;

        var minta = JsonSerializer.Deserialize<List<MintaBejelentes>>(
            await File.ReadAllTextAsync(fajl),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? [];

        var ugyintezok = new List<string>();
        var vegrehajtok = new Dictionary<int, string>();
        foreach (var fiok in DemoFiokok)
        {
            var felhasznalo = await felhasznalok.FindByEmailAsync(fiok.Email);
            if (felhasznalo is null)
                continue;
            if (fiok.Szerepkor == "Ugyintezo")
                ugyintezok.Add(felhasznalo.Id);
            if (fiok.SzervezetId is int szervezetId)
                vegrehajtok[szervezetId] = felhasznalo.Id;
        }

        foreach (var elem in minta)
            await Felvesz(db, elem, mappa, ugyintezok, vegrehajtok);
    }

    private static async Task FiokokLetrehozasa(UserManager<IdentityUser> felhasznalok, ApplicationDbContext db)
    {
        foreach (var fiok in DemoFiokok)
        {
            var felhasznalo = await felhasznalok.FindByEmailAsync(fiok.Email);
            if (felhasznalo is null)
            {
                felhasznalo = new IdentityUser
                {
                    UserName = fiok.Email,
                    Email = fiok.Email,
                    EmailConfirmed = true
                };
                var eredmeny = await felhasznalok.CreateAsync(felhasznalo, DemoJelszo);
                if (!eredmeny.Succeeded)
                    throw new InvalidOperationException($"Nem sikerült létrehozni a demó fiókot: {fiok.Email}");
            }

            if (!await felhasznalok.IsInRoleAsync(felhasznalo, fiok.Szerepkor))
                await felhasznalok.AddToRoleAsync(felhasznalo, fiok.Szerepkor);

            if (fiok.SzervezetId is int szervezetId && !await db.SzervezetiTagok.AnyAsync(t => t.FelhasznaloId == felhasznalo.Id))
            {
                db.SzervezetiTagok.Add(new SzervezetiTag { FelhasznaloId = felhasznalo.Id, SzervezetId = szervezetId });
                await db.SaveChangesAsync();
            }
        }
    }

    private static async Task Felvesz(
        ApplicationDbContext db,
        MintaBejelentes elem,
        string mappa,
        List<string> ugyintezok,
        Dictionary<int, string> vegrehajtok)
    {
        var allapot = Enum.Parse<BejelentesAllapot>(elem.Allapot);
        var veletlen = new Random(int.Parse(elem.Azonosito[3..]));
        var beerkezes = elem.Bekuldes!.Value.Date.AddMinutes(veletlen.Next(7 * 60, 21 * 60));
        var ellenorzes = elem.Ellenorzes?.Date;
        var lezaras = elem.Lezaras?.Date;
        var ugyintezo = ugyintezok.Count > 0 ? ugyintezok[veletlen.Next(ugyintezok.Count)] : null;

        var bejelentes = new Bejelentes
        {
            Cim = elem.Cim,
            Leiras = elem.Leiras,
            Helyszin = elem.Helyszin,
            LetrehozvaUtc = beerkezes,
            Kategoria = Enum.Parse<BejelentesKategoria>(elem.Kategoria),
            Allapot = allapot,
            Hatarido = elem.Hatarido?.Date,
            Kovetokod = Guid.Parse(elem.Kovetokod),
            Ertekeles = elem.Ertekeles,
            Szelesseg = elem.Szelesseg,
            Hosszusag = elem.Hosszusag,
            Veszelyes = elem.Veszelyes
        };
        db.Bejelentesek.Add(bejelentes);

        var reszfeladatok = new List<Reszfeladat>();
        foreach (var rf in elem.Reszfeladatok)
        {
            var reszfeladat = new Reszfeladat
            {
                Bejelentes = bejelentes,
                SzervezetId = SzervezetAzonositok[rf.Szervezet],
                Cim = rf.Cim,
                Hatarido = rf.Hatarido!.Value.Date,
                Elkeszult = rf.Kesz,
                Megjegyzes = string.IsNullOrWhiteSpace(rf.Megjegyzes) ? null : rf.Megjegyzes
            };
            reszfeladatok.Add(reszfeladat);
            db.Reszfeladatok.Add(reszfeladat);
        }

        var naplo = new List<BejelentesNaplo>
        {
            Sor(bejelentes, "Bejelentés érkezett", null, BejelentesAllapot.Uj, null, beerkezes, null)
        };

        if (allapot is BejelentesAllapot.Teves or BejelentesAllapot.HataskoronKivuli)
        {
            var szoveg = allapot == BejelentesAllapot.Teves ? "Téves bejelentés: " : "Hatáskörön kívüli ügy: ";
            naplo.Add(Sor(bejelentes, Rovidit(szoveg + elem.Indok, 100), BejelentesAllapot.Uj, allapot, null, ellenorzes!.Value.AddHours(8), ugyintezo));
        }
        else if (allapot != BejelentesAllapot.Uj)
        {
            naplo.Add(Sor(bejelentes, "Ellenőrzés és határidő beállítása", BejelentesAllapot.Uj, BejelentesAllapot.Ellenorizve, bejelentes.Hatarido, ellenorzes!.Value.AddHours(8), ugyintezo));
        }

        if (allapot is BejelentesAllapot.Folyamatban or BejelentesAllapot.LezarasraVar or BejelentesAllapot.Lezart)
        {
            var rf = reszfeladatok[0];
            naplo.Add(Sor(bejelentes, $"Feladat kiosztva: szervezet #{rf.SzervezetId}, határidő {rf.Hatarido:yyyy-MM-dd}", BejelentesAllapot.Ellenorizve, BejelentesAllapot.Folyamatban, bejelentes.Hatarido, ellenorzes!.Value.AddHours(9), ugyintezo));

            if (allapot is BejelentesAllapot.LezarasraVar or BejelentesAllapot.Lezart)
            {
                var kesz = allapot == BejelentesAllapot.Lezart
                    ? lezaras!.Value.AddHours(10)
                    : Kisebb(ellenorzes.Value.AddDays(3).AddHours(10), DateTime.UtcNow.AddHours(-1));
                if (kesz <= ellenorzes.Value.AddHours(9))
                    kesz = ellenorzes.Value.AddHours(10);
                var vegrehajto = vegrehajtok.GetValueOrDefault(rf.SzervezetId);
                naplo.Add(Sor(bejelentes, "Részfeladat elkészült: fotók: 0", BejelentesAllapot.Folyamatban, BejelentesAllapot.LezarasraVar, bejelentes.Hatarido, kesz, vegrehajto));
            }
        }

        if (allapot == BejelentesAllapot.Lezart)
        {
            var lezarasIdo = lezaras!.Value.AddHours(11);
            bejelentes.LezarvaUtc = lezarasIdo;
            naplo.Add(Sor(bejelentes, "Ügyintéző ellenőrizte és lezárta az ügyet", BejelentesAllapot.LezarasraVar, BejelentesAllapot.Lezart, bejelentes.Hatarido, lezarasIdo, ugyintezo));

            if (elem.Ertekeles is int pont)
                naplo.Add(Sor(bejelentes, $"Lakossági értékelés: {pont}/5", BejelentesAllapot.Lezart, BejelentesAllapot.Lezart, bejelentes.Hatarido, lezaras.Value.AddHours(12), null));
        }

        db.BejelentesNaplok.AddRange(naplo);

        foreach (var nev in elem.Kepek)
        {
            var ut = Path.Combine(mappa, "kepek", nev);
            if (!File.Exists(ut))
                continue;

            db.BejelentesFotok.Add(new BejelentesFoto
            {
                Bejelentes = bejelentes,
                Tipus = BejelentesFotoTipus.Bejelentes,
                TartalomTipus = "image/jpeg",
                Tartalom = await File.ReadAllBytesAsync(ut)
            });
        }

        await db.SaveChangesAsync();
    }

    private static BejelentesNaplo Sor(
        Bejelentes bejelentes,
        string muvelet,
        BejelentesAllapot? elozo,
        BejelentesAllapot uj,
        DateTime? hatarido,
        DateTime idopont,
        string? felhasznaloId) => new()
    {
        Bejelentes = bejelentes,
        Muvelet = Rovidit(muvelet, 100),
        ElozoAllapot = elozo,
        UjAllapot = uj,
        Hatarido = hatarido,
        IdopontUtc = idopont,
        FelhasznaloId = felhasznaloId
    };

    private static string Rovidit(string szoveg, int hossz) =>
        szoveg.Length <= hossz ? szoveg : szoveg[..hossz];

    private static DateTime Kisebb(DateTime a, DateTime b) => a < b ? a : b;

    private record DemoFiok(string Email, string Szerepkor, int? SzervezetId);

    private class MintaBejelentes
    {
        public string Azonosito { get; set; } = string.Empty;
        public string Kovetokod { get; set; } = string.Empty;
        public string Cim { get; set; } = string.Empty;
        public string Leiras { get; set; } = string.Empty;
        public string Helyszin { get; set; } = string.Empty;
        public double? Szelesseg { get; set; }
        public double? Hosszusag { get; set; }
        public string Kategoria { get; set; } = string.Empty;
        public DateTime? Bekuldes { get; set; }
        public DateTime? Ellenorzes { get; set; }
        public string Allapot { get; set; } = string.Empty;
        public bool Veszelyes { get; set; }
        public DateTime? Hatarido { get; set; }
        public DateTime? Lezaras { get; set; }
        public int? Ertekeles { get; set; }
        public string? Indok { get; set; }
        public List<string> Kepek { get; set; } = [];
        public List<MintaReszfeladat> Reszfeladatok { get; set; } = [];
    }

    private class MintaReszfeladat
    {
        public string Szervezet { get; set; } = string.Empty;
        public string Cim { get; set; } = string.Empty;
        public DateTime? Hatarido { get; set; }
        public bool Kesz { get; set; }
        public string? Megjegyzes { get; set; }
    }
}
