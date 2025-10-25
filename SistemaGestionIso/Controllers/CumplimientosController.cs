using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SistemaGestionIso.Controllers
{
    public class CumplimientosController : Controller
    {
        // GET: CumplimientosController
        public ActionResult Index()
        {
            return View();
        }

        // GET: CumplimientosController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CumplimientosController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CumplimientosController/Create
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

        // GET: CumplimientosController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CumplimientosController/Edit/5
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

        // GET: CumplimientosController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CumplimientosController/Delete/5
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
