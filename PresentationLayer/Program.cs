using Serilog;
using BusinessLayer.Abstract;
using BusinessLayer.Concrete;
using DataAccessLayer.Abstract;
using DataAccessLayer.Context;
using DataAccessLayer.EntityFramework;
using ElmahCore.Mvc;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using PresentationLayer.Models;
using System.Text.Json.Serialization;

// Configuración inicial de Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug() // Nivel mínimo de log (Debug para el desarrollo)
    .WriteTo.Console() // Registrar logs en la consola
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day) // Guardar logs diarios en la carpeta Logs
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Configurar Serilog como el sistema de logging principal
builder.Host.UseSerilog();

// Configuración de servicios
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<DataAccesContext>();

builder.Services.AddScoped<IUsuarioDal, EfUsuario>();
builder.Services.AddScoped<IUsuarioService, UsuarioManager>();

builder.Services.AddScoped<IRolDal, EfRol>();
builder.Services.AddScoped<IRolService, RolService>();

builder.Services.AddScoped<IAuthDal, EfAuth>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IToolsDal, EfTools>();
builder.Services.AddScoped<IToolsService, ToolsService>();

builder.Services.AddScoped<IBriefDal, EfBrief>();
builder.Services.AddScoped<IBriefService, BriefService>();

// Configuración de EmailSettings
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.Configure<CategoriaCorreo>(builder.Configuration.GetSection("CategoriasDeCorreo"));

builder.Services.AddScoped<EmailSender>();
builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.AddAuthentication("MyCookieAuthenticationScheme")
    .AddCookie("MyCookieAuthenticationScheme", options =>
    {
        options.LoginPath = "/Login/Index"; // Redirigir a esta ruta cuando no esté autenticado
        options.AccessDeniedPath = "/Login/AccessDenied"; // Opcional: ruta para acceso denegado
        options.ExpireTimeSpan = TimeSpan.FromHours(1); // Duración de la sesión (1 hora)
        options.SlidingExpiration = true; // Renueva la expiración si el usuario está activo
    });

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthorization();

// Configuración de JSON para evitar ciclos de referencia
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });


builder.Services.AddElmah(options =>
{
    options.Path = "/elmah";
});

var app = builder.Build();

// Configuración de middlewares
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
/*
app.Use(async (context, next) =>
{
    if (!context.User.Identity.IsAuthenticated && !context.Request.Path.StartsWithSegments("/Login"))
    {
        if (!context.Request.Path.StartsWithSegments("/Usuarios/SolicitudUsuario") &&
            !context.Request.Path.StartsWithSegments("/Usuarios/CambioContrasena"))
        {
            context.Response.Redirect("/Login");
            return;
        }
    }
    else if (context.User.Identity.IsAuthenticated && context.Request.Path.StartsWithSegments("/Login") &&
             !context.Request.Path.Equals("/Login/Logout"))
    {
        context.Response.Redirect("/Home");
        return;
    }

    await next();
});
*/
app.MapControllers();
app.UseElmah();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
