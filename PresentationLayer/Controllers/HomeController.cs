using BusinessLayer.Abstract;
using BusinessLayer.Concrete;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Models;
using System;
using System.Diagnostics;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace PresentationLayer.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IToolsService _toolsService;

        private readonly ILogger<HomeController> _logger;


        public HomeController(ILogger<HomeController> logger, IAuthService authService, IToolsService toolsService)
        {
            _logger = logger;
            _authService = authService;
            _toolsService = toolsService;

        }
        [Authorize]
        public IActionResult TestAuth()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return Content("Usuario autenticado");
            }
            else
            {
                return Content("No autenticado");
            }
        }
        public ActionResult Index()
        {
            IEnumerable<Menu> menus = null;
            
            if (User?.Identity?.IsAuthenticated == true)
            {
                ViewBag.RolId = Int32.Parse(User.FindFirst(ClaimTypes.Role)?.Value);
                ViewBag.Email = User.FindFirst(ClaimTypes.Email)?.Value;
                ViewBag.Name = User.FindFirst(ClaimTypes.Name)?.Value;
                ViewBag.UsuarioId = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                ViewBag.Menus = _authService.GetMenusByRole(ViewBag.RolId);
                ViewBag.Alertas = _toolsService.ObtenerAlertaUsuario(ViewBag.UsuarioId);
                ViewBag.ConteoAlertas = _toolsService.GetUnreadAlertsCount(ViewBag.UsuarioId);
            }
            else
            {
                RedirectToAction("Index", "Login");
            }

            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public ActionResult ObtenerAlertas()
        {
            respuestaServicio res = new respuestaServicio();
            
            try
            {
                var UsuarioId = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var alertas = _toolsService.ObtenerAlertas(UsuarioId);

                res.Datos = alertas;
                res.Mensaje = "Solicitud Exitosa";
                res.Exito = true;
            }
            catch (Exception ex)
            {
                res.Mensaje = "Petición fallida";
                res.Exito = false;
            }
            return Ok(res);
        }
    }
}
