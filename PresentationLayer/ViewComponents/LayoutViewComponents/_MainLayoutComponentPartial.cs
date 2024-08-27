using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.ViewComponents.LayoutViewComponents
{
    public class _MainLayoutComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
