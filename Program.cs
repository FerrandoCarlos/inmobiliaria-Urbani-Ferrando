using InmobiliariaApp.Repositories.Interfaces;
using InmobiliariaApp.Repositories.Implementations;
using MySqlConnector;
using InmobiliariaApp.Services.Interfaces;
using InmobiliariaApp.Services.Implementations;
using Microsoft.AspNetCore.Authentication.Cookies;
using InmobiliariaApp.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Autenticación por cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

// Registro de repositorios y servicios (Inyección de Dependencias).
// AddScoped: una instancia por request HTTP - correcto para trabajar
// con conexiones a base de datos (ni Singleton ni Transient).
builder.Services.AddScoped<IPropietarioRepository, PropietarioRepository>();
builder.Services.AddScoped<IPropietarioService, PropietarioService>();
builder.Services.AddScoped<IInquilinoRepository, InquilinoRepository>();
builder.Services.AddScoped<IInquilinoService, InquilinoService>();
builder.Services.AddScoped<IInmuebleRepository, InmuebleRepository>();
builder.Services.AddScoped<IInmuebleService, InmuebleService>();
builder.Services.AddScoped<IReservaRepository, ReservaRepository>();
builder.Services.AddScoped<IReservaService, ReservaService>();
builder.Services.AddScoped<IImagenesInmuebleRepository, ImagenesInmuebleRepository>();
builder.Services.AddScoped<IImagenesInmuebleService, ImagenesInmuebleService>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IPagoRepository, PagoRepository>();
builder.Services.AddScoped<IPagoService, PagoService>();
builder.Services.AddScoped<ITipoInmuebleRepository, TipoInmuebleRepository>();
builder.Services.AddScoped<ITipoInmuebleService, TipoInmuebleService>();
builder.Services.AddScoped<IInmuebleFiltroRepository, InmuebleFiltroRepository>();
builder.Services.AddScoped<IInmuebleFiltroService, InmuebleFiltroService>();
builder.Services.AddScoped<IReservaFiltroRepository, ReservaFiltroRepository>();
builder.Services.AddScoped<IReservaFiltroService, ReservaFiltroService>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");

    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets().AllowAnonymous();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Seed inicial:crea un Admin y un empleado con la DB vacía. si hay datos no hace nada
using (var scope = app.Services.CreateScope())
{
    var usuarioService = scope.ServiceProvider.GetRequiredService<IUsuarioService>();
    var usuarioRepo = scope.ServiceProvider.GetRequiredService<IUsuarioRepository>();

    if (usuarioRepo.ObtenerCantidad() == 0)
    {
        var admin = new Usuario
        {
            Email = "admin@inmobiliaria.com",
            Nombre = "Admin",
            Apellido = "Sistema",
            RolId = 1
        };
        usuarioService.Alta(admin, "Admin123!");

        var empleado = new Usuario
        {
            Email = "empleado@inmobiliaria.com",
            Nombre = "Empleado",
            Apellido = "Sistema",
            RolId = 2
        };
        usuarioService.Alta(empleado, "Empleado123!");
    }
}
app.Run();
