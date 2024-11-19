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
                ViewBag.ConteoAlertas = _toolsService.GetUnreadAlertsCount(ViewBag.UsuarioId);
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
                   


                    var urlBase = $"{Request.Scheme}://{Request.Host}";
                    // Diccionario con los valores dinámicos a reemplazar
                    var valoresDinamicos = new Dictionary<string, string>()
                {
                    { "estatus", EstatusMaterial.Descripcion},
                    { "nombreMaterial", material.Nombre },
                    { "link", urlBase + "/Materiales"  }
                };
                    _emailSender.SendEmail(Destinatarios, "ComentarioMaterial", valoresDinamicos);
                }
                
               

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
public async Task<IActionResult> UploadImage(IFormFile upload)
{
    if (upload == null || upload.Length == 0)
    {
        return BadRequest(new { error = "No se recibió ningún archivo para cargar." });
    }

    // Generar un nombre único para el archivo
    var fileName = Path.GetRandomFileName() + Path.GetExtension(upload.FileName);

    // Define la ruta donde guardar el archivo
    var path = Path.Combine("wwwroot/uploads", fileName);

    // Crear el directorio si no existe
    if (!Directory.Exists("wwwroot/uploads"))
    {
        Directory.CreateDirectory("wwwroot/uploads");
    }

    // Guardar el archivo
    using (var stream = new FileStream(path, FileMode.Create))
    {
        await upload.CopyToAsync(stream);
    }

    // Crear la URL para acceder al archivo cargado
    var url = Url.Content($"~/uploads/{fileName}");

    // Devolver la URL en el formato esperado por CKEditor
    return Json(new { url });
}
    }
}