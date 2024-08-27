using Microsoft.AspNetCore.Mvc;
namespace PresentationLayer.ViewComponents.LayoutViewComponents
{
    public class _HeaderLayoutComponentPartial:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
