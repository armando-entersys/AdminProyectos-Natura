using BusinessLayer.Abstract;
using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PresentationLayer.Models;
using System.Security.Claims;


namespace PresentationLayer.Controllers
{

    public class BriefController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IEmailSender _emailSender;
        private readonly IBriefService _briefService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IToolsService _toolsService;
        public BriefController(IEmailSender emailSender, IBriefService briefService, IAuthService authService, IWebHostEnvironment hostingEnvironment, IToolsService toolsService)
        {
            _emailSender = emailSender;
            _briefService = briefService;
            _authService = authService;
            _hostingEnvironment = hostingEnvironment;
            _toolsService = toolsService;
        }
        // GET: BriefController
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
        public ActionResult IndexAdmin()
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
        public IActionResult GetAllColumns()
        {
            int UsuarioId = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            respuestaServicio res = new respuestaServicio();
            var columns = _briefService.GetColumnsByUserId(UsuarioId);
            res.Datos = columns;
            res.Exito = true;

            return Ok(res);
        }
        [HttpGet]
        public IActionResult GetAllbyUserId()
        {
            int UsuarioId = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            respuestaServicio res = new respuestaServicio();
            res.Datos = _briefService.GetAllbyUserId(UsuarioId);
            res.Exito = true;

            return Ok(res);
        }
        // GET: BriefController/Details/5
        public ActionResult Details(int id)
        {
            respuestaServicio res = new respuestaServicio();
            var brief = _briefService.GetById(id);
            res.Datos = brief;
            res.Exito = true;


            return Ok(res);
        
        }
        [HttpGet]
        public IActionResult DownloadFile(int id)
        {
            var brief = _briefService.GetById(id);

            if (brief == null || string.IsNullOrEmpty(brief.RutaArchivo))
            {
                return NotFound("Archivo no encontrado.");
            }

            // Obtener la ruta del archivo en el servidor
            var filePath = Path.Combine(_hostingEnvironment.WebRootPath + "\\uploads\\Brief\\" + brief.Id, brief.RutaArchivo);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Archivo no encontrado.");
            }

            // Obtener el tipo MIME del archivo
            var mimeType = GetMimeType(filePath);

            // Devolver el archivo para descargar
            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, mimeType, Path.GetFileName(filePath));
        }

        // Método auxiliar para determinar el tipo MIME basado en la extensión del archivo
        private string GetMimeType(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            return extension switch
            {
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                _ => "application/octet-stream",
            };
        }

        [HttpPost]
        public ActionResult Create([FromBody] Brief brief)
        {
            respuestaServicio res = new respuestaServicio();

            brief.FechaModificacion = DateTime.Now;
            brief.FechaRegistro = DateTime.Now;
        
           // _emailSender.SendEmailAsync(usuario.Correo, "Bienvenido a Administrador de Proyectos", "<h1>Gracias por unirte a MyApp</h1>");
            res.Mensaje = "Brief agregado exitosamente";
            res.Exito = true;
            return Ok(res);
        }

        [HttpPut]
        public ActionResult EditStatus([FromBody] Brief brief)
        {
            respuestaServicio res = new respuestaServicio();
            var BriefOrg = _briefService.GetById(brief.Id);
            brief.UsuarioId = BriefOrg.UsuarioId;
            brief.Comentario = BriefOrg.Comentario;
            brief.Nombre = BriefOrg.Nombre;
            brief.Objetivo = BriefOrg.Objetivo;
            brief.RutaArchivo = BriefOrg.RutaArchivo;
            brief.DirigidoA = BriefOrg.DirigidoA;
            brief.Descripcion = BriefOrg.Descripcion;
            brief.FechaEntrega = BriefOrg.FechaEntrega;
            brief.FechaRegistro = BriefOrg.FechaRegistro;
            brief.FechaModificacion = DateTime.Now;
            brief.TipoBriefId = BriefOrg.TipoBriefId;

            _briefService.Update(brief);
            res.Datos = brief;
            res.Mensaje = "Actualizado exitosamente";
            res.Exito = true;
            return Ok(res);
        }

        [HttpPost]
        public ActionResult AddBrief([FromForm] ArchivoT Addbrief)
        {
            respuestaServicio res = new respuestaServicio();

            if (Addbrief.Archivo != null && (Addbrief.Archivo.ContentType == "application/pdf" ||
                                             Addbrief.Archivo.ContentType == "application/msword" ||
                                             Addbrief.Archivo.ContentType == "application/vnd.openxmlformats-officedocument.wordprocessingml.document"))
            {
                // Guardar el archivo en una ruta específica o procesarlo según sea necesario
                string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "\\uploads\\Brief\\" + Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value));

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string filePath = Path.Combine(uploadsFolder, Addbrief.Archivo.FileName);
                Addbrief.RutaArchivo = Addbrief.Archivo.FileName;
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    Addbrief.Archivo.CopyTo(stream);
                }
            }

            Brief brief = new Brief
            {
                Id = Addbrief.Id,
                UsuarioId = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value),
                Nombre = Addbrief.Nombre,
                Descripcion = Addbrief.Descripcion,
                Objetivo = Addbrief.Objetivo,
                DirigidoA = Addbrief.DirigidoA,
                Comentario = Addbrief.Comentario,
                RutaArchivo = Addbrief.RutaArchivo,
                TipoBriefId = Addbrief.TipoBriefId,
                FechaEntrega = Addbrief.FechaEntrega,
                EstatusBriefId = Addbrief.EstatusBriefId,
                FechaModificacion = DateTime.Now

                // Puedes asignar más propiedades de `Addbrief` si es necesario
            };

            _briefService.Insert(brief);
            //Envio Correo
            
            // Diccionario con los valores dinámicos a reemplazar
            var valoresDinamicos = new Dictionary<string, string>()
            {
                { "nombre", User.FindFirst(ClaimTypes.Name)?.Value },
                { "nombreBreaf", brief.Nombre }
            };
            var Destinatarios = _toolsService.GetUsuarioByRol(1).Select(q=> q.Correo).ToList();

            _emailSender.SendEmail(Destinatarios, "NuevoBreaf", valoresDinamicos);

           
            res.Datos = brief;
            res.Mensaje = "Se ha recibido correctamente tu solicitud. En breve recibirás una notificación del estatus de tu solicitud.";
            res.Exito = true;

            return Ok(res);
        }
        [HttpPost]
        public ActionResult EditBrief([FromForm] ArchivoT Addbrief)
        {
            respuestaServicio res = new respuestaServicio();
            Brief briefOld = _briefService.GetById(Addbrief.Id);
            if (Addbrief.Archivo != null && (Addbrief.Archivo.ContentType == "application/pdf" ||
                                             Addbrief.Archivo.ContentType == "application/msword" ||
                                             Addbrief.Archivo.ContentType == "application/vnd.openxmlformats-officedocument.wordprocessingml.document"))
            {
                // Guardar el archivo en una ruta específica o procesarlo según sea necesario
                string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "\\uploads\\Brief\\" + Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value));

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string filePath = Path.Combine(uploadsFolder, Addbrief.Archivo.FileName);
                Addbrief.RutaArchivo = Addbrief.Archivo.FileName;
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    Addbrief.Archivo.CopyTo(stream);
                }
            }
            else
            {
                Addbrief.RutaArchivo = briefOld.RutaArchivo;
            }

            Brief brief = new Brief
            {
                Id = Addbrief.Id,
                UsuarioId = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value),
                Nombre = Addbrief.Nombre,
                Descripcion = Addbrief.Descripcion,
                Objetivo = Addbrief.Objetivo,
                DirigidoA = Addbrief.DirigidoA,
                Comentario = Addbrief.Comentario,
                RutaArchivo = Addbrief.RutaArchivo,
                TipoBriefId = Addbrief.TipoBriefId,
                FechaEntrega = Addbrief.FechaEntrega,
                EstatusBriefId = Addbrief.EstatusBriefId,
                FechaModificacion = DateTime.Now

                // Puedes asignar más propiedades de `Addbrief` si es necesario
            };

            _briefService.Update(brief);
            //Envio Correo

            // Diccionario con los valores dinámicos a reemplazar
            var valoresDinamicos = new Dictionary<string, string>()
            {
                { "nombre", User.FindFirst(ClaimTypes.Name)?.Value },
                { "nombreBreaf", brief.Nombre }
            };
            var Destinatarios = _toolsService.GetUsuarioByRol(1).Select(q => q.Correo).ToList();

            _emailSender.SendEmail(Destinatarios, "EdicionBreaf", valoresDinamicos);


            res.Datos = brief;
            res.Mensaje = "Se ha recibido correctamente tu solicitud. En breve recibirás una notificación del estatus de tu solicitud.";
            res.Exito = true;

            return Ok(res);
        }

        [HttpGet]
        public IActionResult GetAllEstatusBrief()
        {
            respuestaServicio res = new respuestaServicio();
            var roles = _briefService.GetAllEstatusBrief();
            res.Datos = roles;
            res.Exito = true;


            return Ok(res);
        }
        [HttpGet]
        public IActionResult GetAllTipoBrief()
        {
            respuestaServicio res = new respuestaServicio();
            var roles = _briefService.GetAllTipoBrief();
            res.Datos = roles;
            res.Exito = true;


            return Ok(res);
        }

        [HttpPost]
        public ActionResult CreateProyecto([FromBody] Proyecto proyecto)
        {
            respuestaServicio res = new respuestaServicio();

            proyecto.FechaModificacion = DateTime.Now;
            try
            {
                _briefService.InsertProyecto(proyecto);
                res.Mensaje = "Creado exitosamente";
                res.Exito = true;
            }
            catch (Exception ex)
            {
                res.Mensaje = "Error al Crear el Usuario";
                res.Exito = false;
            }
            
            return Ok(res);
        }
    }
}
