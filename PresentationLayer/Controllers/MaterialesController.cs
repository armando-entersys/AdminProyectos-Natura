using BusinessLayer.Abstract;
using BusinessLayer.Concrete;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Models;
using System.Security.Claims;

namespace PresentationLayer.Controllers
{
    public class MaterialesController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IBriefService _briefService;


        public MaterialesController(IEmailSender emailSender, IAuthService authService, IWebHostEnvironment hostingEnvironment, IBriefService briefService)
        {
            _emailSender = emailSender;
            _authService = authService;
            _hostingEnvironment = hostingEnvironment;
            _briefService = briefService;

        }
        public IActionResult Index()
        {
            IEnumerable<Menu> menus = null;

            if (User?.Identity?.IsAuthenticated == true)
            {
                ViewBag.RolId = Int32.Parse(User.FindFirst(ClaimTypes.Role)?.Value);
                ViewBag.Email = User.FindFirst(ClaimTypes.Email)?.Value;
                ViewBag.Name = User.FindFirst(ClaimTypes.Name)?.Value;
                ViewBag.UsuarioId = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                ViewBag.Menus = _authService.GetMenusByRole(ViewBag.RolId);
            }
            else
            {
                RedirectToAction("Index", "Login");
            }
            return View();
        }
        [HttpGet]
        public ActionResult ObtenerMateriales()
        {
            respuestaServicio res = new respuestaServicio();
            try
            {
                var id = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var materiales = _briefService.GetMaterialesByUser(id);
                res.Datos = materiales;
                res.Exito = true;
            }
            catch (Exception)
            {
                res.Mensaje = "Petición fallida";
                res.Exito = false;
            }

            return Ok(res);

        }
        [HttpPost]
        public ActionResult ObtenerMaterialesPorNombre([FromBody] Material material)
        {
            respuestaServicio res = new respuestaServicio();
            try
            {
                var id = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                material.Id = id;
                var materiales = _briefService.GetMaterialesFilter(material);
                res.Datos = materiales;
                res.Exito = true;
            }
            catch (Exception)
            {

                res.Mensaje = "Petición fallida";
                res.Exito = false;
            }


            return Ok(res);

        }
        [HttpGet]
        public ActionResult ObtenerConteoEstatusMateriales()
        {
            respuestaServicio res = new respuestaServicio();
            try
            {
                var id = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var materiales = _briefService.ObtenerConteoEstatusMateriales(id);
                res.Datos = materiales;
                res.Exito = true;
            }
            catch (Exception)
            {

                res.Mensaje = "Petición fallida";
                res.Exito = false;
            }


            return Ok(res);

        }
        [HttpGet]
        public ActionResult ObtenerEstatusMateriales()
        {
            respuestaServicio res = new respuestaServicio();
            try
            {
                var materiales = _briefService.GetAllEstatusMateriales();
                res.Datos = materiales;
                res.Exito = true;
            }
            catch (Exception)
            {

                res.Mensaje = "Petición fallida";
                res.Exito = false;
            }


            return Ok(res);

        }
        [HttpPost]
        public ActionResult ActualizarMaterial([FromBody] HistorialMaterial historialMaterial)
        {
            respuestaServicio res = new respuestaServicio();
            try
            {
                var id = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
               _briefService.ActualizaHistorialMaterial(historialMaterial);
                res.Exito = true;
            }
            catch (Exception)
            {

                res.Mensaje = "Petición fallida";
                res.Exito = false;
            }


            return Ok(res);

        }
    }
}