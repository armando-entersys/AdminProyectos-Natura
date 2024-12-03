using BusinessLayer.Abstract;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Models;
using System.Net;
using System.Security.Claims;

namespace PresentationLayer.Controllers
{
    public class CatalogosController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IToolsService _toolService;

        /*private readonly IGenericService<Audiencia> _AudienciaService;
        private readonly IGenericService<TipoBreaf> _TipoBriefService;
        private readonly IGenericService<TipoAlerta> _TipoAlertaService;
        private readonly IGenericService<Prioridad> _PrioridadService;
        private readonly IGenericService<PCN> _PCNService;
        private readonly IGenericService<EstatusMaterial> _EstatusMaterialService;
        private readonly IGenericService<EstatusBreaf> _EstatusBriefService;
        private readonly IGenericService<Formato> _FormatoService;
        */


        public CatalogosController(IToolsService toolService, IAuthService authService)
                                       /*, IGenericService<Audiencia> AudicenciaService)
            
                                   IGenericService<TipoBreaf> TipoBriefService, IGenericService<TipoAlerta> TipoAlertaService,
                                   IGenericService<Prioridad> PrioridadService, IGenericService<PCN> PCNService,
                                   IGenericService<EstatusMaterial> EstatusMaterialService, IGenericService<EstatusBreaf> EstatusBriefService,
                                   IGenericService<Formato> FormatoService)
            */
        {
            _toolService = toolService;
            _authService = authService;
            /*  _AudienciaService = AudicenciaService;
          _TipoBriefService = TipoBriefService;
            _TipoAlertaService = TipoAlertaService;
            _PrioridadService = PrioridadService;
            _PCNService = PCNService;
            _EstatusMaterialService = EstatusMaterialService;
            _EstatusBriefService = EstatusBriefService;
            _FormatoService = FormatoService;
          */

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
        public IActionResult GetAudiencia()
        {
            respuestaServicio res = new respuestaServicio();
            try
            {
                var obj = _toolService.GetAllAudiencia();
                res.Datos = obj;
                res.Exito = true;


            }
            catch (Exception ex)
            {
                res.Exito = false;
                res.Mensaje = "Error al ejecutar solicitud";

            }
            return Ok(res);
        }
        [HttpPost]
        public ActionResult CreateAudiencia([FromBody] Audiencia audiencia)
        {
            respuestaServicio res = new respuestaServicio();
            try
            {
                if (audiencia.Id != 0)
                {
                    _toolService.UpdateAudiencia(audiencia);
                }
                else
                {
                    _toolService.InsertAudiencia(audiencia);
                }
                res.Mensaje = "Solicitud exitosa";
                res.Exito = true;


            }
            catch (Exception ex)
            {
                res.Exito = false;
                res.Mensaje = "Error al ejecutar solicitud";

            }
            return Ok(res);
        }

        [HttpPost]
        public ActionResult Details([FromBody] PeticionCatalogos peticionCatalogos)
        {
            respuestaServicio res = new respuestaServicio();

            try
            {
                if (peticionCatalogos.nombreCatalogo == "Audiencia")
                {
                    res.Datos = _toolService.GetByAudienciaId(peticionCatalogos.Id);
                }
                else if (peticionCatalogos.nombreCatalogo == "TipoBreaf")
                {
                    res.Datos = _toolService.GetByAudienciaId(peticionCatalogos.Id);
                }
                
                res.Mensaje = "Solicitud exitosa";
                res.Exito = true;               

            }
            catch (Exception ex)
            {
                res.Exito = false;
                res.Mensaje = "Error al ejecutar solicitud";

            }
            return Ok(res);
        }
        [HttpPost]
        public ActionResult Delete([FromBody] PeticionCatalogos peticionCatalogos)
        {
            respuestaServicio res = new respuestaServicio();
            try
            {
                if (peticionCatalogos.nombreCatalogo == "Audiencia")
                {
                    _toolService.DeleteAudiencia(peticionCatalogos.Id);
                }
                else if (peticionCatalogos.nombreCatalogo == "TipoBreaf")
                {
                    res.Datos = _toolService.GetAllAudiencia();
                }
                
                res.Mensaje = "Solicitud exitosa";
                res.Exito = true;
               

            }
            catch (Exception ex)
            {
                res.Exito=false;
                res.Mensaje = "Error al ejecutar solicitud";
                
            }
            return Ok(res);

        }

        [HttpGet]
        public ActionResult GetCatalogoInfo(string nombreCatalogo)
        {
            respuestaServicio res = new respuestaServicio();

            try
            {
                if(nombreCatalogo == "Audiencia")
                {
                     res.Datos = _toolService.GetAllAudiencia();
                }
                else if (nombreCatalogo == "TipoBreaf")
                {
                    res.Datos = _toolService.GetAllAudiencia();
                }
        
               
                res.Mensaje = "Solicitud exitosa";
                res.Exito = true;

            }
            catch (Exception ex)
            {
                res.Exito = false;
                res.Mensaje = "Error al ejecutar solicitud";

            }
            return Ok(res);
        }

    }
}
