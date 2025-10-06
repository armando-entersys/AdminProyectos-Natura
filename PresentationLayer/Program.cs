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

// Configurar CORS para permitir peticiones desde archivos HTML locales
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

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

            // ═══════════════════════════════════════════════════════════
            // ROLES
            // ═══════════════════════════════════════════════════════════
            var rolAdmin = new Rol { Descripcion = "Administrador" };
            var rolUsuario = new Rol { Descripcion = "Usuario" };
            var rolProduccion = new Rol { Descripcion = "Producción" };
            context.Roles.AddRange(rolAdmin, rolUsuario, rolProduccion);
            context.SaveChanges();
            logger.LogInformation("✅ Roles creados");

            // ═══════════════════════════════════════════════════════════
            // USUARIO ADMIN
            // ═══════════════════════════════════════════════════════════
            var usuarioAdmin = new Usuario
            {
                Nombre = "Admin",
                ApellidoPaterno = "Sistema",
                ApellidoMaterno = "",
                Correo = "ajcortest@gmail.com",
                Contrasena = "Operaciones.2025",
                RolId = rolAdmin.Id,
                Estatus = true,
                FechaRegistro = DateTime.Now,
                FechaModificacion = DateTime.Now,
                CambioContrasena = false,
                SolicitudRegistro = false
            };
            context.Usuarios.Add(usuarioAdmin);
            context.SaveChanges();
            logger.LogInformation("✅ Usuario admin creado");

            // ═══════════════════════════════════════════════════════════
            // MENÚS
            // ═══════════════════════════════════════════════════════════
            var menus = new List<Menu>
            {
                // Menús para Administrador
                new Menu { Nombre = "Home", Ruta = "/Home/Index", Orden = 1, Icono = "lni lni-home", RolId = rolAdmin.Id },
                new Menu { Nombre = "Briefs", Ruta = "/Brief/Index", Orden = 2, Icono = "lni lni-briefcase", RolId = rolAdmin.Id },
                new Menu { Nombre = "Calendario", Ruta = "/Calendario/Index", Orden = 3, Icono = "lni lni-calendar", RolId = rolAdmin.Id },
                new Menu { Nombre = "Materiales", Ruta = "/Materiales/Index", Orden = 4, Icono = "lni lni-files", RolId = rolAdmin.Id },
                new Menu { Nombre = "Alertas", Ruta = "/Alertas/Index", Orden = 5, Icono = "lni lni-alarm", RolId = rolAdmin.Id },
                new Menu { Nombre = "Usuarios", Ruta = "/Usuarios/Index", Orden = 6, Icono = "lni lni-users", RolId = rolAdmin.Id },
                new Menu { Nombre = "Invitaciones", Ruta = "/Invitaciones/Index", Orden = 7, Icono = "lni lni-envelope", RolId = rolAdmin.Id },
                new Menu { Nombre = "Catálogos", Ruta = "/Catalogos/Index", Orden = 8, Icono = "lni lni-list", RolId = rolAdmin.Id },

                // Menús para Usuario
                new Menu { Nombre = "Home", Ruta = "/Home/Index", Orden = 1, Icono = "lni lni-home", RolId = rolUsuario.Id },
                new Menu { Nombre = "Briefs", Ruta = "/Brief/Index", Orden = 2, Icono = "lni lni-briefcase", RolId = rolUsuario.Id },
                new Menu { Nombre = "Materiales", Ruta = "/Materiales/Index", Orden = 3, Icono = "lni lni-files", RolId = rolUsuario.Id },
                new Menu { Nombre = "Calendario", Ruta = "/Calendario/Index", Orden = 4, Icono = "lni lni-calendar", RolId = rolUsuario.Id },
                new Menu { Nombre = "Alertas", Ruta = "/Alertas/Index", Orden = 5, Icono = "lni lni-alarm", RolId = rolUsuario.Id },

                // Menús para Producción
                new Menu { Nombre = "Home", Ruta = "/Home/Index", Orden = 1, Icono = "lni lni-home", RolId = rolProduccion.Id },
                new Menu { Nombre = "Materiales", Ruta = "/Materiales/Index", Orden = 2, Icono = "lni lni-files", RolId = rolProduccion.Id },
                new Menu { Nombre = "Calendario", Ruta = "/Calendario/Index", Orden = 3, Icono = "lni lni-calendar", RolId = rolProduccion.Id },
                new Menu { Nombre = "Alertas", Ruta = "/Alertas/Index", Orden = 4, Icono = "lni lni-alarm", RolId = rolProduccion.Id }
            };
            context.Menus.AddRange(menus);
            context.SaveChanges();
            logger.LogInformation("✅ Menús creados");

            // ═══════════════════════════════════════════════════════════
            // CATÁLOGOS - ESTATUS BRIEF
            // ═══════════════════════════════════════════════════════════
            var estatusBriefs = new List<EstatusBrief>
            {
                new EstatusBrief { Descripcion = "En revisión", Activo = true },
                new EstatusBrief { Descripcion = "Producción", Activo = true },
                new EstatusBrief { Descripcion = "Falta información", Activo = true },
                new EstatusBrief { Descripcion = "Programado", Activo = true }
            };
            context.EstatusBriefs.AddRange(estatusBriefs);
            context.SaveChanges();
            logger.LogInformation("✅ Estatus Brief creados");

            // ═══════════════════════════════════════════════════════════
            // CATÁLOGOS - TIPO BRIEF
            // ═══════════════════════════════════════════════════════════
            var tiposBrief = new List<TipoBrief>
            {
                new TipoBrief { Descripcion = "Campaña Digital", Activo = true },
                new TipoBrief { Descripcion = "Campaña Tradicional", Activo = true },
                new TipoBrief { Descripcion = "Evento", Activo = true },
                new TipoBrief { Descripcion = "Producto", Activo = true },
                new TipoBrief { Descripcion = "Servicio", Activo = true }
            };
            context.TiposBrief.AddRange(tiposBrief);
            context.SaveChanges();
            logger.LogInformation("✅ Tipos Brief creados");

            // ═══════════════════════════════════════════════════════════
            // CATÁLOGOS - PRIORIDAD
            // ═══════════════════════════════════════════════════════════
            var prioridades = new List<Prioridad>
            {
                new Prioridad { Descripcion = "Baja", Activo = true },
                new Prioridad { Descripcion = "Media", Activo = true },
                new Prioridad { Descripcion = "Alta", Activo = true },
                new Prioridad { Descripcion = "Urgente", Activo = true }
            };
            context.Prioridad.AddRange(prioridades);
            context.SaveChanges();
            logger.LogInformation("✅ Prioridades creadas");

            // ═══════════════════════════════════════════════════════════
            // CATÁLOGOS - AUDIENCIA
            // ═══════════════════════════════════════════════════════════
            var audiencias = new List<Audiencia>
            {
                new Audiencia { Descripcion = "General", Activo = true },
                new Audiencia { Descripcion = "Jóvenes (18-25)", Activo = true },
                new Audiencia { Descripcion = "Adultos (26-40)", Activo = true },
                new Audiencia { Descripcion = "Adultos Mayores (41+)", Activo = true },
                new Audiencia { Descripcion = "Empresarial", Activo = true }
            };
            context.Audiencia.AddRange(audiencias);
            context.SaveChanges();
            logger.LogInformation("✅ Audiencias creadas");

            // ═══════════════════════════════════════════════════════════
            // CATÁLOGOS - FORMATO
            // ═══════════════════════════════════════════════════════════
            var formatos = new List<Formato>
            {
                new Formato { Descripcion = "Video", Activo = true },
                new Formato { Descripcion = "Imagen", Activo = true },
                new Formato { Descripcion = "Audio", Activo = true },
                new Formato { Descripcion = "Texto", Activo = true },
                new Formato { Descripcion = "Infografía", Activo = true },
                new Formato { Descripcion = "Banner", Activo = true },
                new Formato { Descripcion = "Post Social Media", Activo = true }
            };
            context.Formato.AddRange(formatos);
            context.SaveChanges();
            logger.LogInformation("✅ Formatos creados");

            // ═══════════════════════════════════════════════════════════
            // CATÁLOGOS - PCN
            // ═══════════════════════════════════════════════════════════
            var pcns = new List<PCN>
            {
                new PCN { Descripcion = "Digital", Activo = true },
                new PCN { Descripcion = "Impreso", Activo = true },
                new PCN { Descripcion = "Audiovisual", Activo = true },
                new PCN { Descripcion = "Web", Activo = true },
                new PCN { Descripcion = "Redes Sociales", Activo = true }
            };
            context.PCN.AddRange(pcns);
            context.SaveChanges();
            logger.LogInformation("✅ PCN creados");

            // ═══════════════════════════════════════════════════════════
            // CATÁLOGOS - ESTATUS MATERIALES
            // ═══════════════════════════════════════════════════════════
            var estatusMateriales = new List<EstatusMaterial>
            {
                new EstatusMaterial { Descripcion = "Pendiente", Activo = true },
                new EstatusMaterial { Descripcion = "En Diseño", Activo = true },
                new EstatusMaterial { Descripcion = "En Revisión", Activo = true },
                new EstatusMaterial { Descripcion = "Aprobado", Activo = true },
                new EstatusMaterial { Descripcion = "En Producción", Activo = true },
                new EstatusMaterial { Descripcion = "Entregado", Activo = true },
                new EstatusMaterial { Descripcion = "Rechazado", Activo = true }
            };
            context.EstatusMateriales.AddRange(estatusMateriales);
            context.SaveChanges();
            logger.LogInformation("✅ Estatus Materiales creados");

            // ═══════════════════════════════════════════════════════════
            // CATÁLOGOS - TIPO ALERTA
            // ═══════════════════════════════════════════════════════════
            var tipoAlertas = new List<TipoAlerta>
            {
                new TipoAlerta { Descripcion = "Información", Activo = true },
                new TipoAlerta { Descripcion = "Advertencia", Activo = true },
                new TipoAlerta { Descripcion = "Error", Activo = true },
                new TipoAlerta { Descripcion = "Crítico", Activo = true }
            };
            context.TipoAlerta.AddRange(tipoAlertas);
            context.SaveChanges();
            logger.LogInformation("✅ Tipos de Alerta creados");

            logger.LogInformation("✅✅✅ SEED COMPLETO: Todos los datos iniciales insertados exitosamente");
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

// HTTPS Redirection deshabilitado cuando está detrás de Traefik
// Traefik maneja SSL/TLS, la app recibe HTTP y responde HTTP
// app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowAll");
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
