using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnkormanyzatiPortal.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = !builder.Environment.IsDevelopment())
    .AddRoles<IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

using (var scope = app.Services.CreateScope())
{
    var szerepkorKezelo = scope.ServiceProvider
        .GetRequiredService<RoleManager<IdentityRole>>();

    foreach (var nev in new[] { "Ugyintezo", "Vegrehajto", "Vezeto" })
    {
        if (!await szerepkorKezelo.RoleExistsAsync(nev))
        {
            var eredmeny = await szerepkorKezelo.CreateAsync(new IdentityRole(nev));
            if (!eredmeny.Succeeded)
                throw new InvalidOperationException($"Nem sikerült létrehozni a szerepkört: {nev}");
        }
    }

    var email = app.Configuration["Fejlesztes:UgyintezoEmail"];

    if (app.Environment.IsDevelopment() && !string.IsNullOrWhiteSpace(email))
    {
        var felhasznaloKezelo = scope.ServiceProvider
            .GetRequiredService<UserManager<IdentityUser>>();

        var fiok = await felhasznaloKezelo.FindByEmailAsync(email);

        if (fiok is not null && !await felhasznaloKezelo.IsInRoleAsync(fiok, "Ugyintezo"))
        {
            var eredmeny = await felhasznaloKezelo.AddToRoleAsync(fiok, "Ugyintezo");
            if (!eredmeny.Succeeded)
                throw new InvalidOperationException("Nem sikerült kiosztani az ügyintézői szerepkört.");
        }
    }
}

app.Run();
