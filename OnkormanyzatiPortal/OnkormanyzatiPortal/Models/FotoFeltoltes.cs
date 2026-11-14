using Microsoft.AspNetCore.Http;

namespace OnkormanyzatiPortal.Models;

public static class FotoFeltoltes
{
    public static async Task<BejelentesFoto?> BeolvasAsync(
        IFormFile? fajl, BejelentesFotoTipus tipus)
    {
        if (fajl is null)
            return null;

        if (fajl.Length < 8 || fajl.Length > 5 * 1024 * 1024)
            throw new ArgumentException("A fénykép nem lehet üres vagy 5 MB-nál nagyobb.");

        using var stream = new MemoryStream();
        await fajl.CopyToAsync(stream);
        var adat = stream.ToArray();

        var png = adat.Take(8).SequenceEqual(
            new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 });

        var jpeg = adat[0] == 255 && adat[1] == 216 && adat[2] == 255
            && adat[^2] == 255 && adat[^1] == 217;

        if (!png && !jpeg)
            throw new ArgumentException("Csak JPEG vagy PNG fénykép tölthető fel.");

        return new BejelentesFoto
        {
            Tipus = tipus,
            TartalomTipus = png ? "image/png" : "image/jpeg",
            Tartalom = adat
        };
    }
}