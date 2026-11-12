using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnkormanyzatiPortal.Models;

namespace OnkormanyzatiPortal.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
        public DbSet<Bejelentes> Bejelentesek { get; set; } = null!;
        public DbSet<BejelentesNaplo> BejelentesNaplok { get; set; } = null!;
    }
}
