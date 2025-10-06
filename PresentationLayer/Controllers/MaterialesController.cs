using BusinessLayer.Abstract;
using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Models;
using System.Security.Claims;

namespace PresentationLayer.Controllers
{
    [Authorize]
    public class MaterialesController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IBriefService _briefService;
        private readonly IUsuarioService _usuarioService;
        private readonly IToolsService _toolsService;
        public MaterialesController(IEmailSender emailSender, IAuthService authService, IWebHostEnvironment hostingEnvironment, IBriefService briefService, IUsuarioService usuarioService, IToolsService toolsService)
        {
            _emailSender = emailSender;
            _authService = authService;
            _hostingEnvironment = hostingEnvironment;
            _briefService = briefService;
            _usuarioService = usuarioService;
            _toolsService = toolsService;
        }
        public IActionResult Index(string filtroNombre = null)
        {
            IEnumerable<Menu> menus = null;

            if (User?.Identity?.IsAuthenticated == true)
            {
                ViewBag.RolId = Int32.Parse(User.FindFirst(ClaimTypes.Role)?.Value);
                ViewBag.Email = User.FindFirst(ClaimTypes.Email)?.Value;
                ViewBag.Name = User.FindFirst(ClaimTypes.Name)?.Value;
                ViewBag.UsuarioId = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                ViewBag.Menus = _authService.GetMenusByRole(ViewBag.RolId);
                ViewBag.ConteoAlertas = _toolsService.GetUnreadAlertsCount(ViewBag.UsuarioId);
                ViewBag.FiltroNombre = filtroNombre;
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
            catch (Exception ex) // Capturamos el error exacto
            {
                res.Mensaje = $"Petición fallida: {ex.Message}";
                res.Exito = false;

                // Registrar detalles del error para la depuración
                Console.WriteLine($"Error en ObtenerMateriales: {ex}");
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
                var EstatusMaterial = _briefService.GetAllEstatusMateriales();
                res.Datos = EstatusMaterial;
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
        public ActionResult AgregarHistorialMaterial([FromBody] AgregarHistorialMaterialRequest historialMaterialRequest)
        {
            respuestaServicio res = new respuestaServicio();
            try
            {
                var UsuarioId = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                historialMaterialRequest.HistorialMaterial.UsuarioId = UsuarioId;
                var id = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var usuarioLogueado =_usuarioService.TGetById(id);  
               _briefService.ActualizaHistorialMaterial(historialMaterialRequest.HistorialMaterial);

                if (historialMaterialRequest.EnvioCorreo)
                {
                    var EstatusMaterial = _briefService.GetAllEstatusMateriales().Where(q => q.Id == historialMaterialRequest.HistorialMaterial.EstatusMaterialId).FirstOrDefault();
                    var material = _briefService.GetMaterial(historialMaterialRequest.HistorialMaterial.MaterialId);
                    var Destinatarios = new List<string>();
                    foreach(var item in historialMaterialRequest.Usuarios)
                    {
                        var usuario = _usuarioService.TGetById(item.Id);
                        Destinatarios.Add(usuario.Correo);
                    }
                    Destinatarios.AddRange(_toolsService.GetUsuarioByRol(3).Select(q => q.Correo).ToList());


                    var urlBase = $"{Request.Scheme}://{Request.Host}";
                    // Diccionario con los valores dinámicos a reemplazar
                    var valoresDinamicos = new Dictionary<string, string>()
                {
                    { "nombreMaterial", material.Nombre},
                    { "usuario",usuarioLogueado.Nombre },
                    { "estatus", EstatusMaterial.Descripcion},
                    { "comentario", historialMaterialRequest.HistorialMaterial.Comentarios },
                    { "link", urlBase + "/Materiales?filtroNombre=" + material.Nombre }
                };
                    _emailSender.SendEmail(Destinatarios, "ComentarioMaterial", valoresDinamicos);
                }

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
        [HttpGet]
        public ActionResult ObtenerHistorial(int id)
        {
            respuestaServicio res = new respuestaServicio();
            try
            {
                var materiales = _briefService.GetAllHistorialMateriales(id);
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
        public IActionResult upload(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var filePath = Path.Combine("wwwroot/uploads", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                var fileUrl = Url.Content($"~/uploads/{fileName}");
                return Json(new { location = fileUrl }); // TinyMCE necesita esta respuesta
            }

            return BadRequest("No se pudo cargar la imagen.");
        }
    }
}