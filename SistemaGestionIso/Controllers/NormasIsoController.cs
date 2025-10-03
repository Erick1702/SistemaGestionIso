using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
using SistemaGestionIso.Entidades;
using SistemaGestionIso.Models;

namespace SistemaGestionIso.Controllers
{
    public class NormasIsoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NormasIsoController(ApplicationDbContext context)
        {
            _context = context;
        }


        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            //return View();

            var normaIsos = await _context.NormaIsos.ToListAsync();

            var viewModel = normaIsos.Select(n => new NormaIsoViewModel
            {
                NormaISOId = n.Id,
                Nombre = n.Nombre,
                Version = n.Version,
                Descripcion = n.Descripcion
            }).ToList();
            return View(viewModel);

        }

        //[AllowAnonymous]
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.NormaIsos.ToListAsync());
        //}

        // GET: Normas/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var norma = await _context.NormaIsos.FirstOrDefaultAsync(m => m.Id == id);
            if (norma == null) return NotFound();

            return View(norma);
        }

        // GET: Normas/Create
        [AllowAnonymous]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Normas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Create(NormaIsoViewModel model)
        {
            if (ModelState.IsValid)
            {
                var norma = new NormaIso
                {
                    Nombre = model.Nombre,
                    Version = model.Version,
                    Descripcion = model.Descripcion
                };

                _context.Add(norma);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }


        // GET: Normas/Edit/5
        [AllowAnonymous]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var norma = await _context.NormaIsos.FindAsync(id);
            if (norma == null) return NotFound();

            var model = new NormaIsoViewModel
            {
                NormaISOId = norma.Id,
                Nombre = norma.Nombre,
                Version = norma.Version,
                Descripcion = norma.Descripcion
            };

            return View(model);
        }

        // POST: Normas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Edit(int id, NormaIsoViewModel model)
        {
            if (id != model.NormaISOId) return NotFound();

            if (ModelState.IsValid)
            {
                var norma = await _context.NormaIsos.FindAsync(id);
                if (norma == null) return NotFound();

                norma.Nombre = model.Nombre;
                norma.Version = model.Version;
                norma.Descripcion = model.Descripcion;

                _context.Update(norma);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }


        // GET: Normas/Delete/5
        [AllowAnonymous]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var norma = await _context.NormaIsos.FirstOrDefaultAsync(m => m.Id == id);
            if (norma == null) return NotFound();

            return View(norma);
        }

        // POST: Normas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var norma = await _context.NormaIsos.FindAsync(id);
            if (norma != null)
            {
                _context.NormaIsos.Remove(norma);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
