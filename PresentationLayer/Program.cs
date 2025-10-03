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
using PresentationLayer.Hubs;
using Microsoft.AspNetCore.SignalR;
using PresentationLayer.Controllers;
using PresentationLayer.Services;
using Microsoft.Extensions.FileProviders;
using Microsoft.EntityFrameworkCore;

// Configuraci�n inicial de Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug() // Nivel m�nimo de log (Debug para el desarrollo)
    .WriteTo.Console() // Registrar logs en la consola
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day) // Guardar logs diarios en la carpeta Logs
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Configurar Serilog como el sistema de logging principal
builder.Host.UseSerilog();

// Configuraci�n de servicios
builder.Services.AddControllersWithViews();

// Configurar DbContext con connection string desde appsettings.json o variables de entorno
builder.Services.AddDbContext<DataAccesContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString);
});

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

// Configuraci�n de EmailSettings
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.Configure<CategoriaCorreo>(builder.Configuration.GetSection("CategoriasDeCorreo"));

builder.Services.AddScoped<EmailSender>();
builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.AddAuthentication("MyCookieAuthenticationScheme")
    .AddCookie("MyCookieAuthenticationScheme", options =>
    {
        options.LoginPath = "/Login/Index"; // Redirigir a esta ruta cuando no est� autenticado
        options.AccessDeniedPath = "/Login/AccessDenied"; // Opcional: ruta para acceso denegado
        options.ExpireTimeSpan = TimeSpan.FromHours(1); // Duraci�n de la sesi�n (1 hora)
        options.SlidingExpiration = true; // Renueva la expiraci�n si el usuario est� activo
    });

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthorization();

// Configuraci�n de JSON para evitar ciclos de referencia
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });


builder.Services.AddElmah(options =>
{
    options.Path = "/elmah";
});

// Agrega SignalR al contenedor de servicios
builder.Services.AddSignalR();
builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
var app = builder.Build();

// ═══════════════════════════════════════════════════════════
// AUTO-CREACIÓN DE BASE DE DATOS (solo en Docker/Staging)
// ═══════════════════════════════════════════════════════════
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<DataAccesContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("Verificando estado de la base de datos...");

        // Crear la base de datos si no existe (incluye todas las tablas)
        if (context.Database.EnsureCreated())
        {
            logger.LogInformation("✅ Base de datos creada exitosamente con todas las tablas");

            // Seed de datos iniciales
            logger.LogInformation("Insertando datos iniciales...");

            // Crear rol Administrador
            var rolAdmin = new Rol
            {
                Descripcion = "Administrador"
            };
            context.Roles.Add(rolAdmin);
            context.SaveChanges();

            // Crear usuario admin
            // Nota: La contraseña se guarda directamente (considera usar hash en producción)
            var usuarioAdmin = new Usuario
            {
                Nombre = "Admin",
                ApellidoPaterno = "Sistema",
                ApellidoMaterno = "",
                Correo = "ajcortest@gmail.com",
                Contrasena = "Operaciones.2025", // IMPORTANTE: En producción usar hash
                RolId = rolAdmin.Id,
                Estatus = true,
                FechaRegistro = DateTime.Now,
                FechaModificacion = DateTime.Now,
                CambioContrasena = false,
                SolicitudRegistro = false
            };
            context.Usuarios.Add(usuarioAdmin);
            context.SaveChanges();

            logger.LogInformation("✅ Datos iniciales insertados: Usuario admin creado");
        }
        else
        {
            logger.LogInformation("ℹ️ Base de datos ya existe");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "❌ Error al crear/verificar la base de datos");
        // No lanzar excepción para permitir que la app siga corriendo
    }
}

// Configuraci�n de middlewares
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

app.MapControllers();
app.UseElmah();
app.MapHub<NotificationHub>("/notificationHub");
app.UsePathBase("/");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
