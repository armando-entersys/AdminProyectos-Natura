using BusinessLayer.Abstract;
using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace PresentationLayer.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IEmailSender _emailSender;
        private readonly IToolsService _toolService;

        public LoginController(IAuthService authService, IEmailSender emailSender, IToolsService toolService)
        {
            _authService = authService;
            _emailSender = emailSender;
            _toolService = toolService;
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
            Password = "Operaciones2024$";

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
                   new Claim(ClaimTypes.Name, usuario.Nombre + " " + usuario.ApellidoPaterno),
                   new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                   new Claim(ClaimTypes.Role, usuario.RolId.ToString())


                };
                var claimsIdentity = new ClaimsIdentity(claims, "MyCookieAuthenticationScheme");
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true, // Mantener la sesión activa
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1) // Expiración en 1 hora
                };

                await HttpContext.SignInAsync("MyCookieAuthenticationScheme", new ClaimsPrincipal(claimsIdentity), authProperties);
                // Redirige al usuario a la vista principal
                return RedirectToAction("Index", "Home");

                /*var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    // Determinar dinámicamente la ruta base
                    var basePath = HttpContext.Request.PathBase.HasValue ? HttpContext.Request.PathBase.Value : "";

                    // Redirigir a la página de inicio con el basePath dinámico
                    return Redirect($"{basePath}/Home");
                }
                else
                {
                    // Determinar dinámicamente la ruta base
                    var basePath = HttpContext.Request.PathBase.HasValue ? HttpContext.Request.PathBase.Value : "";

                    // Redirigir a la página de inicio con el basePath dinámico
                    return Redirect($"{basePath}/Login");
                    
                }
                */
            }


        }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            // Cierra la sesión del usuario actual
            await HttpContext.SignOutAsync("MyCookieAuthenticationScheme");

            // Redirigir a la página de inicio u otra página deseada
            return RedirectToAction("Index", "Home");
        }
        public IActionResult CambioContrasena(int id)
        {
            ViewData["id"] = id;
            var usuario = new Usuario { Id = id }; // Asignamos el ID al modelo
            return View(usuario);
        }
        public IActionResult CambioPassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CambioPasswordEmail(string Correo)
        {

            var urlBase = $"{Request.Scheme}://{Request.Host}";
            var resp =  _authService.CambioPasswordEmail(Correo);
            if (resp.Exito == true)
            {
               

                // Diccionario con los valores dinámicos a reemplazar
                var valoresDinamicos = new Dictionary<string, string>()
                {
                    { "nombre", resp.Datos.Nombre },
                    { "link", urlBase + "/Login/CambioContrasena/"+ resp.Datos.Id }
                };
                List<string> Destinatarios = new List<string>();
                Destinatarios.Add(Correo);

                _emailSender.SendEmail(Destinatarios, "CambioPassword", valoresDinamicos);
            }
            return RedirectToAction("Index", "Home");


        }
        [HttpPost]
        public IActionResult CambiarPasswordUsuario(Usuario usuario)
        {
            var urlBase = $"{Request.Scheme}://{Request.Host}";
            var resp = _authService.CambiarPasswordUsuario(usuario);

            if (resp.Exito)
            {
                var valoresDinamicos = new Dictionary<string, string>()
                    {
                        { "nombre", resp.Datos.Nombre },
                        { "link", urlBase + "/Login" }
                    };

                List<string> Destinatarios = new List<string> { resp.Datos.Correo };
                _emailSender.SendEmail(Destinatarios, "CambioPasswordUsuario", valoresDinamicos);

                TempData["MensajeExito"] = "Contraseña cambiada exitosamente. Serás redirigido en unos momentos.";
                return RedirectToAction("CambioContrasena", new { id = usuario.Id });
            }
            else
            {
                ViewData["Mensaje"] = resp.Mensaje ?? "Error al cambiar la contraseña. Por favor, intenta nuevamente.";
                return View("CambioContrasena", usuario);
            }

            ViewData["Mensaje"] = "Datos no válidos. Verifica los campos.";
            return View("CambioContrasena", usuario);
        }

    }
}
