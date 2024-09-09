using BusinessLayer.Abstract;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Models;
using System.Net;
using System.Security.Claims;
using System.Net.Http;
using BusinessLayer.Concrete;
using Newtonsoft.Json.Linq;
namespace PresentationLayer.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IUsuarioService _usuarioService;
        private readonly IRolService _rolService;
        private readonly IEmailSender _emailSender;
        public UsuariosController(IAuthService authService, IUsuarioService usuarioService, IRolService rolService, IEmailSender emailSender)
        {
            _authService = authService;
            _usuarioService = usuarioService;
            _rolService = rolService;
            _emailSender = emailSender;
        }
        // GET: UsuariosController
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
            var usuarios = _usuarioService.TGetAll()
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
        public IActionResult GetAllRoles()
        {
            respuestaServicio res = new respuestaServicio();
            var roles = _rolService.TGetAll();
            res.Datos = roles;
            res.Exito = true;
        

            return Ok(res);
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            respuestaServicio res = new respuestaServicio();
            var usuario = _usuarioService.TGetById(id);
            res.Datos = usuario;
            res.Exito = true;
            return Ok(res);
        }

        [HttpPost]
        public ActionResult Create([FromBody] Usuario usuario)
        {
            respuestaServicio res = new respuestaServicio();

          
            usuario.UserRol = _rolService.TGetById(usuario.RolId);
            _usuarioService.TInsert(usuario);
            _emailSender.SendEmailAsync(usuario.Correo, "Bienvenido a Administrador de Proyectos", "<h1>Gracias por unirte a MyApp</h1>");
            res.Mensaje = "Usuario agregado exitosamente";
            res.Exito = true;
            return Ok(res);
        }

        [HttpPut]
        public ActionResult Edit([FromBody] Usuario usuario)
        {
            respuestaServicio res = new respuestaServicio();
         
            _usuarioService.TUpdate(usuario);
            res.Datos = usuario;
            res.Mensaje = "Usuario Actualizado exitosamente";
            res.Exito = true;
            return Ok(res);
        }


        [HttpDelete]
        public ActionResult Delete(int id)
        {
            respuestaServicio res = new respuestaServicio();
            _usuarioService.TDelete(id);
            res.Exito = true;
            return Ok(res);
        }

    }
}
