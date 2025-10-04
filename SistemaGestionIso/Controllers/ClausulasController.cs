using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaGestionIso.Entidades;
using SistemaGestionIso.Models;
using SistemaGestionIso.Servicios;

namespace SistemaGestionIso.Controllers
{
    public class ClausulasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClausulasController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        [Authorize(Roles = $"{Constantes.RolAdministrador}, {Constantes.RolEncargadoSig}")]
        public async Task<IActionResult> Listado (int Id, string? mensaje = null)
        {
            var normaIso = await _context.NormaIsos
            .FirstOrDefaultAsync(n => n.Id == Id);

            if (normaIso == null)
            {
                TempData["Error"] = "La norma ISO no existe";
                //return NotFound();
                return RedirectToAction("Crear", "Clausulas");
            }

            var clausulas = await _context.Clausulas
                .Where(c => c.NormaIsoId == Id)
                .OrderBy(c => c.Orden)
                .Select(c => new ClausulaViewModel
                {
                    Id = c.Id,
                    Codigo = c.Codigo,
                    Descripcion = c.Descripcion,
                    NormaIsoId = c.NormaIsoId,
                    NombreNormaIso = c.NormaIso.Nombre,
                    Orden = c.Orden
                })
                .ToListAsync();

            if (clausulas == null || clausulas.Count == 0)
            {
                // No hay cláusulas, vamos a crear la primera
                return RedirectToAction("Crear", new { normaIsoId = Id });
            }

            var viewModel = new ClausulaListadoViewModel
            {
                Clausulas = clausulas,
                Mensaje = TempData["Mensaje"]?.ToString()
            };

            ViewBag.NormaIsoId = Id;
            ViewBag.NombreNormaIso = normaIso.Nombre;
            ViewBag.VersionNormaIso = normaIso.Version;

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Crear(int normaIsoId)
        {
            var normaIso = await _context.NormaIsos
                .FirstOrDefaultAsync(n => n.Id == normaIsoId);

            if (normaIso == null)
            {
                TempData["Error"] = "No se encontró la norma ISO especificada.";
                return RedirectToAction("Index", "NormaIso");
            }

            // 👇 primero calculamos el siguiente número de orden
            int siguienteOrden = await _context.Clausulas
                .Where(c => c.NormaIsoId == normaIso.Id)
                .CountAsync() + 1;

            // 👇 luego inicializamos el ViewModel
            var viewModel = new ClausulaCrearViewModel
            {
                Codigo = string.Empty,
                Descripcion = string.Empty,
                NormaIsoId = normaIso.Id,
                NombreNormaIso = normaIso.Nombre,
                Orden = siguienteOrden
            };

            return View(viewModel);
        }



        [HttpPost]
        public async Task<IActionResult> Crear(ClausulaCrearViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var clausula = new Clausula
            {
                Codigo = modelo.Codigo,
                Descripcion = modelo.Descripcion,
                NormaIsoId = modelo.NormaIsoId,
                Orden = modelo.Orden
            };


            _context.Add(clausula);
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Cláusula creada exitosamente";
            return RedirectToAction("Listado", new { Id = modelo.NormaIsoId });



        }

    }
}
