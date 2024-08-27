using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.ViewComponents.LayoutViewComponents
{
    public class _ScriptLayoutComponentPartial : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
