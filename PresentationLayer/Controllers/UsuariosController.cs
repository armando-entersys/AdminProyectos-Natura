using BusinessLayer.Abstract;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Models;
using System.Net;
using System.Security.Claims;

namespace PresentationLayer.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IUsuarioService _usuarioService;
        public UsuariosController(IAuthService authService, IUsuarioService usuarioService)
        {
            _authService = authService;
            _usuarioService = usuarioService;
        }
        // GET: UsuariosController
        public ActionResult Index()
        {
            IEnumerable<Menu> menus = null;
       
            if (User?.Identity?.IsAuthenticated == true)
            {
                ViewBag.RolId = Int32.Parse(User.FindFirst(ClaimTypes.Role)?.Value);
                ViewBag.Email = User.FindFirst(ClaimTypes.Email)?.Value;
                ViewBag.Name = User.FindFirst(ClaimTypes.Email)?.Value;
                ViewBag.UsuarioId = User.FindFirst(ClaimTypes.Email)?.Value;

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
            var usuarios = _usuarioService.TGetAll();
            res.Datos = usuarios;
            res.Exito = true;
            return Ok(res);

        }

        // GET: UsuariosController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: UsuariosController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UsuariosController/Create
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

        // GET: UsuariosController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UsuariosController/Edit/5
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

        // GET: UsuariosController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UsuariosController/Delete/5
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
