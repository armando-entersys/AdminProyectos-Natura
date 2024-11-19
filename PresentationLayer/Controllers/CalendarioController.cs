using BusinessLayer.Abstract;
using BusinessLayer.Concrete;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PresentationLayer.Controllers
{
    [Authorize]
    public class CalendarioController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IEmailSender _emailSender;
        private readonly IBriefService _briefService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IToolsService _toolsService;
        public CalendarioController(IEmailSender emailSender, IBriefService briefService, IAuthService authService, IWebHostEnvironment hostingEnvironment, IToolsService toolsService)
        {
            _emailSender = emailSender;
            _briefService = briefService;
            _authService = authService;
            _hostingEnvironment = hostingEnvironment;
            _toolsService = toolsService;
        }
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
                ViewBag.ConteoAlertas = _toolsService.GetUnreadAlertsCount(ViewBag.UsuarioId);
            }
            else
            {
                RedirectToAction("Index", "Login");
            }
            return View();
        }
    }
}
