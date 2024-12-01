using BusinessLayer.Abstract;
using BusinessLayer.Concrete;
using DataAccessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace PresentationLayer.Controllers
{
    [Authorize]
    public class InvitacionesController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IToolsService _toolService;
        private readonly IEmailSender _emailSender;
        private readonly IUsuarioService _usuarioService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public InvitacionesController(IToolsService toolService, IAuthService authService, IEmailSender emailSender, IUsuarioService usuarioService, IWebHostEnvironment hostingEnvironment)
        {
            _toolService = toolService;
            _authService = authService;
            _emailSender = emailSender;
            _usuarioService = usuarioService;
            _hostingEnvironment = hostingEnvironment;
        }
        public IActionResult Index()
        {

            if (User?.Identity?.IsAuthenticated == true)
            {
                ViewBag.RolId = Int32.Parse(User.FindFirst(ClaimTypes.Role)?.Value);
                ViewBag.Email = User.FindFirst(ClaimTypes.Email)?.Value;
                ViewBag.Name = User.FindFirst(ClaimTypes.Name)?.Value;
                ViewBag.UsuarioId = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                ViewBag.Menus = _authService.GetMenusByRole(ViewBag.RolId);
                ViewBag.ConteoAlertas = _toolService.GetUnreadAlertsCount(ViewBag.UsuarioId);
            }
            else
            {
                RedirectToAction("Index", "Login");
            }

            return View();
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            respuestaServicio res = new respuestaServicio();
            var usuarios = _toolService.GetUsuarioBySolicitud()
                .Select(q => new Usuario
                {
                    Id = q.Id,
                    Nombre = q.Nombre,
                    ApellidoMaterno = q.ApellidoMaterno,
                    ApellidoPaterno = q.ApellidoPaterno,
                    Correo = q.Correo,
                    Contrasena = q.Contrasena,
                    Estatus = q.Estatus,
                    RolId = q.RolId
                    //UserRol = _rolService.TGetById(q.RolId)
                })
                .ToList();

            res.Datos = usuarios;
            res.Exito = true;
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("This is an example response", System.Text.Encoding.UTF8, "text/plain")
            };
            return Ok(res);
        }

        [HttpGet]
        public ActionResult Aceptar(int id)
        {
            respuestaServicio res = new respuestaServicio();
            res = _toolService.CambioSolicitud(id,true);
            Usuario usuario = _usuarioService.TGetById(id);
            usuario.CambioContrasena = false;
            usuario.Estatus = true;
            _usuarioService.TUpdate(usuario);

            var urlBase = $"{Request.Scheme}://{Request.Host}" + "/AdministradorProyectos";
            var valoresDinamicos = new Dictionary<string, string>
            {
                { "link", urlBase + "/Login" } 
            };
            var Destinatarios = new List<string>();
            Destinatarios.Add(usuario.Correo);

            _emailSender.SendEmail(Destinatarios, "UsuarioAceptado", valoresDinamicos);
            return Ok(res);
        }
        [HttpGet]
        public ActionResult Rechazar(int id)
        {
            respuestaServicio res = new respuestaServicio();
            res = _toolService.CambioSolicitud(id, false);
            Usuario usuario = _usuarioService.TGetById(id);
            usuario.CambioContrasena = false;
            _usuarioService.TDelete(usuario.Id);

            var valoresDinamicos = new Dictionary<string, string>
            {
                { "link","#" }
            };
            var Destinatarios = new List<string>();
            Destinatarios.Add(usuario.Correo);

            _emailSender.SendEmail(Destinatarios, "UsuarioRechazo", valoresDinamicos);
            return Ok(res);
        }

    }
}
