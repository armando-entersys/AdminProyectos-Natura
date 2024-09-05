using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Controllers
{

    public class BriefController : Controller
    {
        // GET: BriefController
        public ActionResult Index()
        {
            return View();
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

        // GET: BriefController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
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
