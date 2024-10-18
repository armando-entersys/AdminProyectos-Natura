using BusinessLayer.Abstract;
using BusinessLayer.Concrete;
using DataAccessLayer.Abstract;
using DataAccessLayer.Context;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using PresentationLayer.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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
// Configurar EmailSettings
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.Configure<CategoriaCorreo>(builder.Configuration.GetSection("CategoriasDeCorreo"));
// Registrar el servicio de envío de correos
builder.Services.AddScoped<EmailSender>();

// Registrar IEmailSender
builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => {
        options.LoginPath = "/";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Solo para HTTPS
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddAuthorization();

// Configurar el ciclo de referencia
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
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
app.UseStaticFiles();
app.UseRouting();
app.UseSession(); // Habilitar el middleware de sesión

app.UseAuthentication();
app.UseAuthorization();
// Middleware personalizado para redirigir usuarios autenticados en la página de login
app.Use(async (context, next) =>
{
    // Verifica si el usuario está autenticado
    if (!context.User.Identity.IsAuthenticated)
    {
        // Si no está autenticado y no intenta acceder a /Login
        if (!context.Request.Path.StartsWithSegments("/Login"))
        {
            if (context.Request.Path.StartsWithSegments("/Usuarios/SolicitudUsuario")|| context.Request.Path.StartsWithSegments("/Usuarios/CambioContrasena"))
            {
                
            }
            else
            {
                context.Response.Redirect("/Login");
                return;
            }

           
        }
    }
    else
    {
        // Si el usuario está autenticado y está intentando acceder a /Login o /Login/Logout
        if (context.Request.Path.StartsWithSegments("/Login"))
        {
            // No redirigir si es el método de Logout
            if (!context.Request.Path.Equals("/Login/Logout"))
            {
                // Redirige al home si ya está autenticado
                context.Response.Redirect("/Home");
                return;
            }
        }
    }

    await next();
});
app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();
