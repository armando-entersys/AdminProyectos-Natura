using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.ViewComponents.LayoutViewComponents
{
    public class _SidebarLayoutComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
