using BusinessLayer.Abstract;
using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PresentationLayer.Hubs;
using PresentationLayer.Models;
using System.Security.Claims;


namespace PresentationLayer.Controllers
{
    [Authorize]
    public class BriefController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IEmailSender _emailSender;
        private readonly IBriefService _briefService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IToolsService _toolsService;
        private readonly IUsuarioService _usuarioService;
        private readonly IHubContext<NotificationHub> _hubContext;
        public BriefController(IEmailSender emailSender, IBriefService briefService, IAuthService authService, 
                                IWebHostEnvironment hostingEnvironment, IToolsService toolsService, 
                                IUsuarioService usuarioService, IHubContext<NotificationHub> hubContext)
        {
            _emailSender = emailSender;
            _briefService = briefService;
            _authService = authService;
            _hostingEnvironment = hostingEnvironment;
            _toolsService = toolsService;
            _usuarioService = usuarioService;
            _hubContext = hubContext;
        }
        // GET: BriefController
        public ActionResult Index(string filtroNombre = null)
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
                ViewBag.ConteoAlertas = _toolsService.GetUnreadAlertsCount(ViewBag.UsuarioId);
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
            res.Datos = _briefService.GetAllbyUserId(UsuarioId,true);
            res.Exito = true;

            return Ok(res);
        }
        [HttpGet]
        public IActionResult GetAllbyUserBrief()
        {
            int UsuarioId = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            respuestaServicio res = new respuestaServicio();
            res.Datos = _briefService.GetAllbyUserId(UsuarioId,false);
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
            brief.LinksReferencias = BriefOrg.LinksReferencias;
            brief.DirigidoA = BriefOrg.DirigidoA;
            brief.Descripcion = BriefOrg.Descripcion;
            brief.FechaEntrega = BriefOrg.FechaEntrega;
            brief.FechaRegistro = BriefOrg.FechaRegistro;
            brief.FechaModificacion = DateTime.Now;
            brief.TipoBriefId = BriefOrg.TipoBriefId;

            _briefService.Update(brief);

            var estatusBriesfs = _briefService.GetAllEstatusBrief();
            brief.EstatusBrief = estatusBriesfs.Where(q => q.Id == brief.EstatusBriefId).FirstOrDefault();

            var Destinatarios = new List<string>();
            var urlBase = $"{Request.Scheme}://{Request.Host}" + "/AdministradorProyectos";

            if (brief.EstatusBriefId == 1)
            {
                Destinatarios = _toolsService.GetUsuarioByRol(1).Select(q => q.Correo).ToList();
                Destinatarios.AddRange(_toolsService.ObtenerParticipantes(brief.Id).Select(q => q.Usuario.Correo).ToList());
            }
            if (brief.EstatusBriefId == 2)
            {
                Destinatarios = _toolsService.GetUsuarioByRol(3).Select(q => q.Correo).ToList();

                Destinatarios.Add(_usuarioService.TGetById(brief.UsuarioId).Correo);

                var usuariosProduccion = _toolsService.GetUsuarioByRol(3).Select(q => q.Id).ToList();
                foreach (var item in usuariosProduccion)
                {
                    _toolsService.CrearAlerta(new Alerta
                    {
                        IdUsuario = item,
                        Nombre = "Cambio Estatus Proyecto " + brief.Nombre,
                        Descripcion = "Cambio de estatus a " + brief.EstatusBrief.Descripcion,
                        IdTipoAlerta = 3,
                        Accion = urlBase + "/Brief?filtroNombre=" + brief.Nombre

                    });
                }
                

            }
            if (brief.EstatusBriefId == 3)
            {
                Destinatarios.Add(_usuarioService.TGetById(brief.UsuarioId).Correo);

            }
            if (brief.EstatusBriefId == 4)
            {
                Destinatarios.Add(_usuarioService.TGetById(brief.UsuarioId).Correo);
            }
            if (brief.EstatusBriefId == 5)
            {
                Destinatarios.Add(_usuarioService.TGetById(brief.UsuarioId).Correo);
            }
            if (brief.EstatusBriefId == 6)
            {
                Destinatarios.Add(_usuarioService.TGetById(brief.UsuarioId).Correo);
            }
            if (brief.EstatusBriefId == 7)
            {
                Destinatarios = _toolsService.GetUsuarioByRol(3).Select(q => q.Correo).ToList();
                Destinatarios.Add(_usuarioService.TGetById(brief.UsuarioId).Correo);
            }
            if (brief.EstatusBriefId == 8)
            {
                Destinatarios = _toolsService.GetUsuarioByRol(3).Select(q => q.Correo).ToList();
                Destinatarios.Add(_usuarioService.TGetById(brief.UsuarioId).Correo);
            }
            _toolsService.CrearAlerta(new Alerta
            {
                IdUsuario = brief.UsuarioId,
                Nombre = "Cambio Estatus Proyecto " + brief.Nombre,
                Descripcion = "Cambio de estatus a " + brief.EstatusBrief.Descripcion,
                IdTipoAlerta = 3,
                Accion = urlBase + "/Brief?filtroNombre=" + brief.Nombre

            });

            var estatusBriefs = _briefService.GetAllEstatusBrief();
            brief.EstatusBrief = estatusBriefs.Where(q => q.Id == brief.EstatusBriefId).FirstOrDefault();
          

            // Diccionario con los valores dinámicos a reemplazar
            var valoresDinamicos = new Dictionary<string, string>()
            {
                { "estatus", brief.EstatusBrief.Descripcion},
                { "nombreProyecto", brief.Nombre },
                { "link", urlBase + "/Brief?filtroNombre=" + brief.Nombre }

            };


            _emailSender.SendEmail(Destinatarios, "ActualizaEstatusProyecto", valoresDinamicos);
            

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
            Addbrief.Comentario = "";
            Brief brief = new Brief
            {
                Id = Addbrief.Id,
                UsuarioId = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value),
                Nombre = Addbrief.Nombre,
                Descripcion = Addbrief.Descripcion,
                Objetivo = Addbrief.Objetivo,
                DirigidoA = Addbrief.DirigidoA,
                Comentario = Addbrief.Comentario,
                LinksReferencias = Addbrief.LinksReferencias,
                RutaArchivo = Addbrief.RutaArchivo,
                TipoBriefId = Addbrief.TipoBriefId,
                FechaEntrega = Addbrief.FechaEntrega,
                EstatusBriefId = Addbrief.EstatusBriefId,
                FechaModificacion = DateTime.Now

                // Puedes asignar más propiedades de `Addbrief` si es necesario
            };

            _briefService.Insert(brief);
            //Envio Correo
            var urlBase = $"{Request.Scheme}://{Request.Host}" + "/AdministradorProyectos";
            // Diccionario con los valores dinámicos a reemplazar
            var valoresDinamicos = new Dictionary<string, string>()
            {
                { "nombre", User.FindFirst(ClaimTypes.Name)?.Value },
                { "nombreProyecto", brief.Nombre },
                { "link", urlBase + "/Brief/IndexAdmin?filtroNombre=" + brief.Nombre }
            };
            var Destinatarios = _toolsService.GetUsuarioByRol(1).Select(q => q.Correo).ToList();

            _emailSender.SendEmail(Destinatarios, "NuevoProyecto", valoresDinamicos);
            
            var usuariosAdmin = _toolsService.GetUsuarioByRol(1).Select(q => q.Id).ToList();
            foreach(var item in usuariosAdmin)
            {
                Alerta alertaUsuario = new Alerta
                {
                    IdUsuario = item,
                    Nombre = "Nuevo Proyecto",
                    Descripcion = "Se agrego un nuevo proyecto " + brief.Nombre,
                    IdTipoAlerta = 1,
                    Accion = urlBase + "/Brief/IndexAdmin?filtroNombre=" + brief.Nombre

                };

                _toolsService.CrearAlerta(alertaUsuario);
            }
           

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
                Comentario = briefOld.Comentario,
                RutaArchivo = Addbrief.RutaArchivo,
                TipoBriefId = Addbrief.TipoBriefId,
                FechaEntrega = Addbrief.FechaEntrega,
                EstatusBriefId = Addbrief.EstatusBriefId,
                FechaModificacion = DateTime.Now,
                LinksReferencias = Addbrief.LinksReferencias
                
                // Puedes asignar más propiedades de `Addbrief` si es necesario
            };

            _briefService.Update(brief);
            //Envio Correo
            var urlBase = $"{Request.Scheme}://{Request.Host}" + "/AdministradorProyectos";
            // Diccionario con los valores dinámicos a reemplazar
            var valoresDinamicos = new Dictionary<string, string>()
            {
                { "nombre", User.FindFirst(ClaimTypes.Name)?.Value },
                { "nombreProyecto", brief.Nombre },
                { "link", urlBase + "/BriefAdmin?filtroNombre=" + brief.Nombre  }

            };
            var Destinatarios = _toolsService.GetUsuarioByRol(1).Select(q => q.Correo).ToList();

            _emailSender.SendEmail(Destinatarios, "EdicionBreaf", valoresDinamicos);


            res.Datos = brief;
            res.Mensaje = "Se ha recibido correctamente tu solicitud.";
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
                res.Datos = _briefService.GetProyectoByBriefId(proyecto.BriefId);
                res.Mensaje = "Creado exitosamente";
                res.Exito = true;
            }
            catch (Exception ex)
            {
                res.Mensaje = "Error al guardar";
                res.Exito = false;
            }

            return Ok(res);
        }
        [HttpPost]
        public ActionResult CreateMaterial([FromBody] Material material)
        {
            respuestaServicio res = new respuestaServicio();
            var urlBase = $"{Request.Scheme}://{Request.Host}" + "/AdministradorProyectos";

            material.FechaModificacion = DateTime.Now;
            material.EstatusMaterialId = 1;
            try
            {
                var UsuarioId = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                
                _briefService.InsertMaterial(material);
                var brief = _briefService.GetById(material.BriefId);
                Alerta alertaUsuario = new Alerta
                {
                    IdUsuario = brief.Id,
                    Nombre = "Nuevo Material",
                    Descripcion = "Se agrego un material al brief " + brief.Nombre,
                    IdTipoAlerta = 4,
                    Accion = urlBase + "/Materiales?filtroNombre="+material.Nombre

                };

                _toolsService.CrearAlerta(alertaUsuario);
                Alerta alertaAdmin = new Alerta
                {
                    IdUsuario = UsuarioId,
                    Nombre = "Nuevo Material",
                    Descripcion = "Se agrego un material al brief " + brief.Nombre,
                    IdTipoAlerta = 4,
                    Accion = urlBase + "/Materiales?filtroNombre=" + material.Nombre

                };

                _toolsService.CrearAlerta(alertaAdmin);

                res.Mensaje = "Creado exitosamente";
                res.Exito = true;
            }
            catch (Exception ex)
            {
                res.Mensaje = "Error al Crear el Material";
                res.Exito = false;
            }

            return Ok(res);
        }
        [HttpGet]
        public ActionResult ObtenerProyectoPorBrief(int id)
        {
            respuestaServicio res = new respuestaServicio();
            var proyecto = _briefService.GetProyectoByBriefId(id);
            res.Datos = proyecto;
            res.Exito = true;

            return Ok(res);

        }
        [HttpGet]
        public ActionResult ObtenerMateriales(int id)
        {
            respuestaServicio res = new respuestaServicio();
            var materiales = _briefService.GetMaterialesByBriefId(id);
            res.Datos = materiales;
            res.Exito = true;

            return Ok(res);

        }
        [HttpGet]
        public ActionResult EliminarMaterial(int id)
        {
            respuestaServicio res = new respuestaServicio();
            
            try
            {
                _briefService.EliminarMaterial(id);
                res.Exito = true;
            }
            catch (Exception ex)
            {
                res.Mensaje = "Error al remover el Material";
                res.Exito = false;
            }
            return Ok(res);

        }
        [HttpGet]
        public ActionResult EliminarParticipante(int id)
        {
            respuestaServicio res = new respuestaServicio();

            try
            {
                _briefService.EliminarParticipante(id);
                res.Exito = true;
            }
            catch (Exception ex)
            {
                res.Mensaje = "Error al remover el Participante";
                res.Exito = false;
            }
            return Ok(res);

        }
        [HttpGet]
        public ActionResult ObtenerConteoPorProyectos()
        {
            respuestaServicio res = new respuestaServicio();

            try
           {
                var UsuarioId = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                res.Datos =_briefService.ObtenerConteoProyectos(UsuarioId);
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
        public ActionResult ObtenerConteoMateriales()
        {
            respuestaServicio res = new respuestaServicio();

            try
            {
                var UsuarioId = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                res.Datos = _briefService.ObtenerConteoMateriales(UsuarioId);
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
        public ActionResult ObtenerConteoProyectoFecha()
        {
            respuestaServicio res = new respuestaServicio();

            try
            {
                var UsuarioId = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                res.Datos = _briefService.ObtenerConteoProyectoFecha(UsuarioId);
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
        public IActionResult GetAllPrioridad()
        {
            respuestaServicio res = new respuestaServicio();
            var data = _briefService.GetAllPrioridades();
            res.Datos = data;
            res.Exito = true;

            return Ok(res);
        }
        [HttpGet]
        public IActionResult GetAllAudiencias()
        {
            respuestaServicio res = new respuestaServicio();
            var data = _briefService.GetAllAudiencias();
            res.Datos = data;
            res.Exito = true;

            return Ok(res);
        }
        [HttpGet]
        public IActionResult GetAllPCN()
        {
            respuestaServicio res = new respuestaServicio();
            var data = _briefService.GetAllPCN();
            res.Datos = data;
            res.Exito = true;

            return Ok(res);
        }
        [HttpGet]
        public IActionResult GetAllFormatos()
        {
            respuestaServicio res = new respuestaServicio();
            var data = _briefService.GetAllFormatos();
            res.Datos = data;
            res.Exito = true;

            return Ok(res);
        }
    }
}
