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
        options.LoginPath = "/Login/Auth";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);

    });
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(
        new ResponseCacheAttribute
        {
            NoStore = true,
            Location = ResponseCacheLocation.None
        });
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
app.UseAuthentication();
app.UseAuthorization();
// Middleware personalizado para redirigir usuarios autenticados en la página de login
app.Use(async (context, next) =>
{
    // Verifica si el usuario está autenticado y está intentando acceder a /Account/Login
    if (context.User.Identity.IsAuthenticated && context.Request.Path.Equals("/Login"))
    {
        // Redirige al home
        context.Response.Redirect("/Home");
        return;
    }

    await next();
});
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();
