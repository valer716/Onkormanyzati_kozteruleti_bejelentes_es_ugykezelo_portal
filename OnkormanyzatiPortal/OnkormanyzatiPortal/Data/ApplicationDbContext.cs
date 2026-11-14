using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnkormanyzatiPortal.Models;

namespace OnkormanyzatiPortal.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
        public DbSet<Bejelentes> Bejelentesek { get; set; } = null!;
        public DbSet<BejelentesNaplo> BejelentesNaplok { get; set; } = null!;
        public DbSet<Szervezet> Szervezetek { get; set; } = null!;
        public DbSet<Reszfeladat> Reszfeladatok { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<SzervezetiTag>()
                .HasOne(t => t.Felhasznalo)
                .WithOne()
                .HasForeignKey<SzervezetiTag>(t => t.FelhasznaloId);

            builder.Entity<Szervezet>().HasData(
                new Szervezet { Id = 1, Nev = "Útfenntartás" },
                new Szervezet { Id = 2, Nev = "Köztisztaság" },
                new Szervezet { Id = 3, Nev = "Közvilágítás" }
            );

            builder.Entity<Bejelentes>()
                .HasIndex(b => b.Kovetokod)
                .IsUnique()
                .HasFilter("[Kovetokod] IS NOT NULL");

            builder.Entity<BejelentesFoto>()
                .HasOne(f => f.Reszfeladat)
                .WithMany()
                .HasForeignKey(f => f.ReszfeladatId)
                .OnDelete(DeleteBehavior.NoAction);
        }
        public DbSet<SzervezetiTag> SzervezetiTagok { get; set; } = null!;
        public DbSet<BejelentesFoto> BejelentesFotok { get; set; } = null!;
    }
}
