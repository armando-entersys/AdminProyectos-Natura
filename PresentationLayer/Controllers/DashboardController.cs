using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
