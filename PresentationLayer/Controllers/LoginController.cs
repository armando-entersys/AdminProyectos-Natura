using BusinessLayer.Abstract;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
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
        public async Task<IActionResult> Autenticar(string Username, string Password)
        {
           // Username = "ajcortest@gmail.com";
            Password = "Operaciones.2024";

            Usuario usuario = await _authService.Autenticar(Username, Password);
            
         

            if (usuario == null)
            {
                ViewData["Mensaje"] = "Error de Autenticación";
                return View("Index");
            }
            else
            {
                List<Claim> claims = new List<Claim>()
                {
                   new Claim(ClaimTypes.Email, usuario.Correo),
                   new Claim(ClaimTypes.Name, usuario.Nombre + usuario.ApellidoPaterno),
                   new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                   new Claim(ClaimTypes.Role, usuario.RolId.ToString())


                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
                return RedirectToAction("Index", "Home");
            }

            
        }
    }
}
