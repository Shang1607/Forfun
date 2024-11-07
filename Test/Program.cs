using Microsoft.EntityFrameworkCore;
using Test.Data;

var builder = WebApplication.CreateBuilder(args);

// Legg til tjenester i containeren.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

// **Legg til Session-tjenesten**
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Sett session timeout hvis ønskelig
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Konfigurer HTTP-forespørselsrøret.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // Standard HSTS-verdi er 30 dager. Du kan endre dette for produksjonsscenarier.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// **Legg til Session-middleware før Authorization**
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
