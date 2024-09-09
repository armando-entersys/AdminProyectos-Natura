using BusinessLayer.Abstract;
using BusinessLayer.Concrete;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Models;
using System.Net;
using System.Security.Claims;

namespace PresentationLayer.Controllers
{

    public class BriefController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IEmailSender _emailSender;
        private readonly IBriefService _briefService;

        public BriefController(IEmailSender emailSender, IBriefService briefService, IAuthService authService)
        {
            _emailSender = emailSender;
            _briefService = briefService;
            _authService = authService;
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
        // GET: BriefController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: BriefController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BriefController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
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

            _briefService.Update(brief);
            res.Datos = brief;
            res.Mensaje = "Actualizado exitosamente";
            res.Exito = true;
            return Ok(res);
        }

        // POST: BriefController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: BriefController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: BriefController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
