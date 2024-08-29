using BusinessLayer.Abstract;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PresentationLayer.Controllers
{
    public class LoginController : Controller
    {
        private readonly IAuthService _authService;

        public LoginController(IAuthService authService)
        {
            _authService = authService;
        }
        public IActionResult Registro()
        {
            return View();
        }
        public IActionResult Index()
        {
            
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Auth(string Username, string Password)
        {
            Usuario usuario = await _authService.Autenticar(Username, Password);
            if (usuario == null)
            {
                ViewData["Mensaje"] = "Error de Autenticación";
                return View("Index");
            }
            List<Claim> claims = new List<Claim>()
            {
               new Claim(ClaimTypes.Email, usuario.Correo),
               new Claim(ClaimTypes.Name, usuario.Nombre + usuario.ApellidoPaterno),
               new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
               new Claim(ClaimTypes.Role, usuario.UserRol.Descripcion.ToString())


            };
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true
            };
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                properties);
            return RedirectToAction("Index", "Home");
        }
    }
}
