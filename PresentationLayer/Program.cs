using BusinessLayer.Abstract;
using BusinessLayer.Concrete;
using DataAccessLayer.Abstract;
using DataAccessLayer.Context;
using DataAccessLayer.EntityFramework;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<DataAccesContext>();

builder.Services.AddScoped<IUsuarioDal, EfUsuario>();
builder.Services.AddScoped<IUsuarioService, UsuarioManager>();

builder.Services.AddScoped<IAuthDal, EfAuth>();
builder.Services.AddScoped<IAuthService, AuthService>();

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
    // Verifica si el usuario está autenticado y está intentando acceder a /Account/Login
    if (context.User.Identity.IsAuthenticated && (context.Request.Path.Equals("/Login") || context.Request.Path.Equals("/")))
    {
        // Redirige al home
        context.Response.Redirect("/Home");
        return;
    }
    /*if (!context.User.Identity.IsAuthenticated && !context.Request.Path.Equals("/Login"))
    {
        // Redirige al home
        context.Response.Redirect("/Login");
        return;
    }*/
    await next();
});
app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();
