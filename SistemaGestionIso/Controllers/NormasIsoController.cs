using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
using SistemaGestionIso.Entidades;
using SistemaGestionIso.Models;
using SistemaGestionIso.Servicios;


namespace SistemaGestionIso.Controllers
{
    public class NormasIsoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NormasIsoController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> Detalle(int id)
        {
            var norma = await _context.NormaIsos
                .FirstOrDefaultAsync(n => n.Id == id);
            if (norma is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            
            var modelo = new NormaIsoDetalleViewModel
            {
                Id = norma.Id,
                Nombre = norma.Nombre,
                Version = norma.Version,
                Descripcion = norma.Descripcion
            };
            return View(modelo);
        }


        [HttpGet]
        [Authorize(Roles = Constantes.RolAdministrador)]
        public async Task<IActionResult> Listado(string? mensaje = null)
        {
            var normasIso = await _context.NormaIsos.Select(x => new NormaIsoViewModel
            {
                NormaISOId = x.Id,
                Nombre = x.Nombre,
                Version = x.Version,
                Descripcion = x.Descripcion
            }).ToListAsync();

            var modelo = new NormaIsoListadoViewModel();

            modelo.NormasIso = normasIso;
            modelo.Mensaje = mensaje;
            return View(modelo);
        }


        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles =$"{Constantes.RolAdministrador}, {Constantes.RolEncargadoSig}")]
        public async Task<IActionResult> Crear(NormaIsoCrearViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var normaiso = new NormaIso
            {
                Nombre = modelo.Nombre,
                Version = modelo.Version,
                Descripcion = modelo.Descripcion
            };

            _context.Add(normaiso);
            await _context.SaveChangesAsync();

            return RedirectToAction("Detalle", new {id = normaiso.Id });
        }






        // GET: Normas/Edit/5
        [HttpGet]
        [Authorize(Roles =$"{Constantes.RolAdministrador},{Constantes.RolEncargadoSig}")]
        public async Task<IActionResult> Editar(int? id)
        {
            

            var norma = await _context.NormaIsos.FirstOrDefaultAsync(x => x.Id == id);
            if (norma is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var model = new NormaIsoEditarViewModel
            {
                Id = norma.Id,
                Nombre = norma.Nombre,
                Version = norma.Version,
                Descripcion = norma.Descripcion
            };

            return View(model);
        }

        // POST: Normas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{Constantes.RolAdministrador},{Constantes.RolEncargadoSig}")]
        public async Task<IActionResult> Editar ( NormaIsoEditarViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var norma = await _context.NormaIsos.FirstOrDefaultAsync(x => x.Id == modelo.Id);

            if (norma is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            norma.Nombre = modelo.Nombre;
            norma.Version = modelo.Version;
            norma.Descripcion = modelo.Descripcion;

            await _context.SaveChangesAsync();

            return RedirectToAction("Detalle", new { id = norma.Id });
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
