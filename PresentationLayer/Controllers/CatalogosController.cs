using BusinessLayer.Abstract;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PresentationLayer.Models;
using System.Dynamic;
using System.Net;
using System.Security.Claims;
using System.Text.Json;

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
        public ActionResult Create([FromBody] PeticionCatalogos peticionCatalogos)
        {
            respuestaServicio res = new respuestaServicio();
            try
            {
          
                if (peticionCatalogos.Objeto is System.Text.Json.JsonElement jsonElement)
                {
                    if (peticionCatalogos.nombreCatalogo == "Audiencia")
                    {
                        var obj = System.Text.Json.JsonSerializer.Deserialize<Audiencia>(jsonElement.GetRawText());

                        if (peticionCatalogos.Id != 0)
                        {
                            _toolService.UpdateAudiencia(obj);
                        }
                        else
                        {
                            _toolService.InsertAudiencia(obj);
                        }
                    }
                    else if (peticionCatalogos.nombreCatalogo == "TipoBrief")
                    {
                        var obj = System.Text.Json.JsonSerializer.Deserialize<TipoBrief>(jsonElement.GetRawText());
                    

                        if (peticionCatalogos.Id != 0)
                        {
                            _toolService.UpdateTipoBrief(obj);
                        }
                        else
                        {
                            _toolService.InsertTipoBrief(obj);
                        }

                        res.Mensaje = "Solicitud exitosa";
                        res.Exito = true;


                    }
                    else if (peticionCatalogos.nombreCatalogo == "TipoAlerta")
                    {
                        var obj = System.Text.Json.JsonSerializer.Deserialize<TipoAlerta>(jsonElement.GetRawText());


                        if (peticionCatalogos.Id != 0)
                        {
                            _toolService.UpdateTipoAlerta(obj);
                        }
                        else
                        {
                            _toolService.InsertTipoAlerta(obj);
                        }

                        res.Mensaje = "Solicitud exitosa";
                        res.Exito = true;


                    }
                    else if (peticionCatalogos.nombreCatalogo == "Prioridad")
                    {
                        var obj = System.Text.Json.JsonSerializer.Deserialize<Prioridad>(jsonElement.GetRawText());


                        if (peticionCatalogos.Id != 0)
                        {
                            _toolService.UpdatePrioridad(obj);
                        }
                        else
                        {
                            _toolService.InsertPrioridad(obj);
                        }

                        res.Mensaje = "Solicitud exitosa";
                        res.Exito = true;


                    }
                    else if (peticionCatalogos.nombreCatalogo == "PCN")
                    {
                        var obj = System.Text.Json.JsonSerializer.Deserialize<PCN>(jsonElement.GetRawText());


                        if (peticionCatalogos.Id != 0)
                        {
                            _toolService.UpdatePCN(obj);
                        }
                        else
                        {
                            _toolService.InsertPCN(obj);
                        }

                        res.Mensaje = "Solicitud exitosa";
                        res.Exito = true;


                    }
                    else if (peticionCatalogos.nombreCatalogo == "EstatusMaterial")
                    {
                        var obj = System.Text.Json.JsonSerializer.Deserialize<EstatusMaterial>(jsonElement.GetRawText());


                        if (peticionCatalogos.Id != 0)
                        {
                            _toolService.UpdateEstatusMaterial(obj);
                        }
                        else
                        {
                            _toolService.InsertEstatusMaterial(obj);
                        }

                        res.Mensaje = "Solicitud exitosa";
                        res.Exito = true;


                    }
                    else if (peticionCatalogos.nombreCatalogo == "EstatusBrief")
                    {
                        var obj = System.Text.Json.JsonSerializer.Deserialize<EstatusBrief>(jsonElement.GetRawText());


                        if (peticionCatalogos.Id != 0)
                        {
                            _toolService.UpdateEstatusBrief(obj);
                        }
                        else
                        {
                            _toolService.InsertEstatusBrief(obj);
                        }

                        res.Mensaje = "Solicitud exitosa";
                        res.Exito = true;


                    }
                    else if (peticionCatalogos.nombreCatalogo == "Formato")
                    {
                        var obj = System.Text.Json.JsonSerializer.Deserialize<Formato>(jsonElement.GetRawText());


                        if (peticionCatalogos.Id != 0)
                        {
                            _toolService.UpdateFormato(obj);
                        }
                        else
                        {
                            _toolService.InsertFormato(obj);
                        }

                        res.Mensaje = "Solicitud exitosa";
                        res.Exito = true;


                    }
                }
               
            }
            catch (Exception ex)
            {
                res.Exito = false;
                res.Mensaje = "Error al ejecutar solicitud";

            }
            return Ok(res);
        }
        // Método para obtener el valor de una propiedad de un objeto dinámico
        private object GetDynamicPropertyValue(dynamic obj, string propertyName)
        {
            var property = obj.GetType().GetProperty(propertyName);
            if (property != null)
            {
                return property.GetValue(obj);
            }
            else
            {
                // Para tipos dinámicos como ExpandoObject
                if (obj is IDictionary<string, object> dict && dict.ContainsKey(propertyName))
                {
                    return dict[propertyName];
                }
            }
            throw new InvalidOperationException($"La propiedad '{propertyName}' no existe en el objeto dinámico.");
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
        public ActionResult CreateTipoBrief([FromBody] TipoBrief tipoBrief)
        {
            respuestaServicio res = new respuestaServicio();
            try
            {
                if (tipoBrief.Id != 0)
                {
                    _toolService.UpdateTipoBrief(tipoBrief);
                }
                else
                {
                    _toolService.InsertTipoBrief(tipoBrief);
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
        public ActionResult CreateTipoAlerta([FromBody] TipoAlerta tipoAlerta)
        {
            respuestaServicio res = new respuestaServicio();
            try
            {
                if (tipoAlerta.Id != 0)
                {
                    _toolService.UpdateTipoAlerta(tipoAlerta);
                }
                else
                {
                    _toolService.InsertTipoAlerta(tipoAlerta);
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
        public ActionResult CreatePrioridad([FromBody] Prioridad prioridad)
        {
            respuestaServicio res = new respuestaServicio();
            try
            {
                if (prioridad.Id != 0)
                {
                    _toolService.UpdatePrioridad(prioridad);
                }
                else
                {
                    _toolService.InsertPrioridad(prioridad);
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
        public ActionResult CreatePCN([FromBody] PCN PCN)
        {
            respuestaServicio res = new respuestaServicio();
            try
            {
                if (PCN.Id != 0)
                {
                    _toolService.UpdatePCN(PCN);
                }
                else
                {
                    _toolService.InsertPCN(PCN);
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
        public ActionResult CreateEstatusMaterial([FromBody] EstatusMaterial estatusMaterial)
        {
            respuestaServicio res = new respuestaServicio();
            try
            {
                if (estatusMaterial.Id != 0)
                {
                    _toolService.UpdateEstatusMaterial(estatusMaterial);
                }
                else
                {
                    _toolService.InsertEstatusMaterial(estatusMaterial);
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
        public ActionResult CreateEstatusBrief([FromBody] EstatusBrief estatusBrief)
        {
            respuestaServicio res = new respuestaServicio();
            try
            {
                if (estatusBrief.Id != 0)
                {
                    _toolService.UpdateEstatusBrief(estatusBrief);
                }
                else
                {
                    _toolService.InsertEstatusBrief(estatusBrief);
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
                else if (peticionCatalogos.nombreCatalogo == "TipoBrief")
                {
                    res.Datos = _toolService.GetByTipoBriefId(peticionCatalogos.Id);
                }
                else if (peticionCatalogos.nombreCatalogo == "TipoAlerta")
                {
                    res.Datos = _toolService.GetByTipoAlertaId(peticionCatalogos.Id);
                }
                else if (peticionCatalogos.nombreCatalogo == "Prioridad")
                {
                    res.Datos = _toolService.GetByPrioridadId(peticionCatalogos.Id);
                }
                else if (peticionCatalogos.nombreCatalogo == "PCN")
                {
                    res.Datos = _toolService.GetByPCNId(peticionCatalogos.Id);
                }
                else if (peticionCatalogos.nombreCatalogo == "EstatusMaterial")
                {
                    res.Datos = _toolService.GetByEstatusMaterialId(peticionCatalogos.Id);
                }
                else if (peticionCatalogos.nombreCatalogo == "EstatusBrief")
                {
                    res.Datos = _toolService.GetByEstatusBriefId(peticionCatalogos.Id);
                }
                else if (peticionCatalogos.nombreCatalogo == "Formato")
                {
                    res.Datos = _toolService.GetByFormatoId(peticionCatalogos.Id);
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
                else if (peticionCatalogos.nombreCatalogo == "TipoBrief")
                {
                    _toolService.DeleteTipoBrief(peticionCatalogos.Id);
                }
                else if (peticionCatalogos.nombreCatalogo == "TipoAlerta")
                {
                    _toolService.DeleteTipoAlerta(peticionCatalogos.Id);
                }
                else if (peticionCatalogos.nombreCatalogo == "Prioridad")
                {
                    _toolService.DeletePrioridad(peticionCatalogos.Id);
                }
                else if (peticionCatalogos.nombreCatalogo == "PCN")
                {
                    _toolService.DeletePCN(peticionCatalogos.Id);
                }
                else if (peticionCatalogos.nombreCatalogo == "EstatusMaterial")
                {
                    _toolService.DeleteEstatusMaterial(peticionCatalogos.Id);
                }
                else if (peticionCatalogos.nombreCatalogo == "EstatusBrief")
                {
                    _toolService.DeleteEstatusBrief(peticionCatalogos.Id);
                }
                else if (peticionCatalogos.nombreCatalogo == "Formato")
                {
                    _toolService.DeleteFormato(peticionCatalogos.Id);
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
                else if (nombreCatalogo == "TipoBrief")
                {
                    res.Datos = _toolService.GetAllTipoBrief();
                }
                else if (nombreCatalogo == "TipoAlerta")
                {
                    res.Datos = _toolService.GetAllTipoAlerta();
                }
                else if (nombreCatalogo == "Prioridad")
                {
                    res.Datos = _toolService.GetAllPrioridad();
                }
                else if (nombreCatalogo == "PCN")
                {
                    res.Datos = _toolService.GetAllPCN();
                }
                else if (nombreCatalogo == "EstatusMaterial")
                {
                    res.Datos = _toolService.GetAllEstatusMaterial();
                }
                else if (nombreCatalogo == "EstatusBrief")
                {
                    res.Datos = _toolService.GetAllEstatusBrief();
                }
                else if (nombreCatalogo == "Formato")
                {
                    res.Datos = _toolService.GetAllFormato();
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
