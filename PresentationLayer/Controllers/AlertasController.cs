using BusinessLayer.Abstract;
using BusinessLayer.Concrete;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Models;
using System.Security.Claims;

namespace PresentationLayer.Controllers
{
    [Authorize]
    public class AlertasController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IUsuarioService _usuarioService;
        private readonly IRolService _rolService;
        private readonly IEmailSender _emailSender;
        private readonly IToolsService _toolService;
        private readonly IGenericService<Alerta> _GenericService;

        public AlertasController(IAuthService authService, IRolService rolService, IEmailSender emailSender, IToolsService toolService)
        {
            _authService = authService;
            _rolService = rolService;
            _emailSender = emailSender;
            _toolService = toolService;
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
                ViewBag.ConteoAlertas = _toolService.GetUnreadAlertsCount(ViewBag.UsuarioId);
            }
            else
            {
                RedirectToAction("Index", "Login");
            }

            return View();
        }
        public ActionResult ObtenerAlertas()
        {
            respuestaServicio res = new respuestaServicio();

            try
            {
                var UsuarioId = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                res.Datos = _toolService.ObtenerAlertas(UsuarioId);
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
        public ActionResult ObtenerTiposAlerta()
        {
            respuestaServicio res = new respuestaServicio();

            try
            {
                res.Datos = _toolService.ObtenerTiposAlerta();
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
        public ActionResult ActualizarAlerta(int id)
        {
            respuestaServicio res = new respuestaServicio();

            try
            {
                _toolService.UpdateAlerta(id);
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

    [ApiController]
    [Route("api/[controller]")]
    public class AlertsController : ControllerBase
    {
        private readonly IToolsService _toolService;
        public AlertsController(IToolsService toolService)
        {
            _toolService = toolService;
        }

        public ActionResult GetUnreadAlertsCount()
        {
            respuestaServicio res = new respuestaServicio();

            try
            {
                // Obtener el ID del usuario desde los claims (similar a ObtenerAlertas)
                var UsuarioId = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                // Llamar al servicio que obtiene el conteo de alertas no leídas para el usuario
                var unreadCount = _toolService.GetUnreadAlertsCount(UsuarioId);

                // Construir la respuesta
                res.Datos = unreadCount;
                res.Mensaje = "Solicitud Exitosa";
                res.Exito = true;
            }
            catch (Exception ex)
            {
                // Si ocurre un error, capturarlo y devolver la respuesta con error
                res.Mensaje = "Petición fallida";
                res.Exito = false;
            }

            // Retornar la respuesta al cliente
            return Ok(res);
        }
    }
}
