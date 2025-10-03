using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using SistemaGestionIso;
using SistemaGestionIso.Entidades;

var builder = WebApplication.CreateBuilder(args);

var politicaUsuarioAutenticados = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();

// Add services to the container.
builder.Services.AddControllersWithViews(opciones =>
{
    opciones.Filters.Add(new AuthorizeFilter(politicaUsuarioAutenticados));
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer("name=DefaultConnection"));

//Identity
builder.Services.AddAuthentication();

builder.Services.AddIdentity<Usuario, Rol>(opciones =>
{
    opciones.SignIn.RequireConfirmedAccount = false;
    opciones.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
    opciones.Lockout.MaxFailedAccessAttempts = 3;
    opciones.Lockout.AllowedForNewUsers = true;
}
).AddRoles<Rol>()
 .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

///


builder.Services.PostConfigure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme, opciones =>
{
    opciones.LoginPath = "/Usuarios/Login";
    opciones.AccessDeniedPath = "/Usuarios/AccesoDenegado";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
