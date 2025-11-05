using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaGestionIso.Entidades;
using SistemaGestionIso.Models;
using SistemaGestionIso.Models.Requisito;

using SistemaGestionIso.Servicios;
using System.Runtime.CompilerServices;

namespace SistemaGestionIso.Controllers
{
    public class RequisitosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RequisitosController(ApplicationDbContext context)
        {
            _context = context;
        }




        // GET: Requisitos (Todos los requisitos)
        [HttpGet]
        [Authorize(Roles = $"{Constantes.RolAdministrador}, {Constantes.RolEncargadoSig}")]
        public async Task<IActionResult> Index()
        {
            var requisitos = await _context.Requisitos
                .Include(r => r.Clausula)
                    .ThenInclude(c => c.NormaIso)
                .OrderBy(r => r.Clausula.NormaIso.Nombre)
                    .ThenBy(r => r.Clausula.Orden)
                    .ThenBy(r => r.Id)
                .Select(r => new RequisitoViewModel
                {
                    Id = r.Id,
                    Descripcion = r.Descripcion,
                    ClausulaId = r.ClausulaId,
                    CodigoClausula = r.Clausula.Codigo,
                    DescripcionClausula = r.Clausula.Descripcion,
                    NombreNormaIso = r.Clausula.NormaIso.Nombre + " - " + r.Clausula.NormaIso.Version,
                    NormaIsoId = r.Clausula.NormaIsoId,
                    CantidadCumplimientos = r.Cumplimientos.Count,
                    CantidadHallazgos = r.Hallazgos.Count
                })
                .ToListAsync();

            var viewModel = new RequisitoListadoViewModel
            {
                Requisitos = requisitos,
                Mensaje = TempData["Mensaje"]?.ToString()
            };

            return View(viewModel);
        }

        // GET: Requisitos/Listado/5 (Requisitos de una cláusula específica)
        [HttpGet]
        [Authorize(Roles = $"{Constantes.RolAdministrador}, {Constantes.RolEncargadoSig}")]
        public async Task<IActionResult> Listado(int id)
        {
            var clausula = await _context.Clausulas
                .Include(c => c.NormaIso)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (clausula == null)
            {
                TempData["Error"] = "La cláusula no existe";
                return RedirectToAction("Index", "Clausulas");
            }

            var requisitos = await _context.Requisitos
                .Where(r => r.ClausulaId == id)
                .Select(r => new RequisitoViewModel
                {
                    Id = r.Id,
                    Descripcion = r.Descripcion,
                    ClausulaId = r.ClausulaId,
                    CodigoClausula = r.Clausula.Codigo,
                    DescripcionClausula = r.Clausula.Descripcion,
                    NombreNormaIso = r.Clausula.NormaIso.Nombre + " - " + r.Clausula.NormaIso.Version,
                    NormaIsoId = r.Clausula.NormaIsoId,
                    CantidadCumplimientos = r.Cumplimientos.Count,
                    CantidadHallazgos = r.Hallazgos.Count
                })
                .ToListAsync();

            var viewModel = new RequisitoListadoViewModel
            {
                Requisitos = requisitos,
                Mensaje = TempData["Mensaje"]?.ToString()
            };

            ViewBag.ClausulaId = id;
            ViewBag.CodigoClausula = clausula.Codigo;
            ViewBag.DescripcionClausula = clausula.Descripcion;
            ViewBag.NombreNormaIso = clausula.NormaIso.Nombre;
            ViewBag.VersionNormaIso = clausula.NormaIso.Version;
            ViewBag.NormaIsoId = clausula.NormaIsoId;

            return View(viewModel);
        }

        // GET: Requisitos/Crear?clausulaId=5
        [HttpGet]
        [Authorize(Roles = $"{Constantes.RolAdministrador}, {Constantes.RolEncargadoSig}")]
        public async Task<IActionResult> Crear(int clausulaId)
        {
            var clausula = await _context.Clausulas
                .Include(c => c.NormaIso)
                .FirstOrDefaultAsync(c => c.Id == clausulaId);

            if (clausula == null)
            {
                TempData["Error"] = "La cláusula no existe";
                return RedirectToAction("Index", "Clausulas");
            }

            var viewModel = new RequisitoCrearViewModel
            {
                ClausulaId = clausulaId,
                CodigoClausula = clausula.Codigo,
                DescripcionClausula = clausula.Descripcion,
                NombreNormaIso = $"{clausula.NormaIso.Nombre} - {clausula.NormaIso.Version}",
                NormaIsoId = clausula.NormaIsoId
            };

            return View(viewModel);
        }


        // POST: Requisitos/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{Constantes.RolAdministrador}, {Constantes.RolEncargadoSig}")]
        public async Task<IActionResult> Crear(RequisitoCrearViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                // Recargar información de la cláusula si hay errores
                var clausulaError = await _context.Clausulas
                    .Include(c => c.NormaIso)
                    .FirstOrDefaultAsync(c => c.Id == modelo.ClausulaId);

                if (clausulaError != null)
                {
                    modelo.CodigoClausula = clausulaError.Codigo;
                    modelo.DescripcionClausula = clausulaError.Descripcion;
                    modelo.NombreNormaIso = $"{clausulaError.NormaIso.Nombre} - {clausulaError.NormaIso.Version}";
                    modelo.NormaIsoId = clausulaError.NormaIsoId;
                }

                return View(modelo);
            }

            try
            {
                var requisito = new Requisito
                {
                    Descripcion = modelo.Descripcion,
                    ClausulaId = modelo.ClausulaId
                };

                _context.Requisitos.Add(requisito);
                await _context.SaveChangesAsync();

                TempData["Mensaje"] = "Requisito creado exitosamente";
                return RedirectToAction(nameof(Listado), new { id = modelo.ClausulaId });
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException?.Message.Contains("truncated") == true)
                {
                    ModelState.AddModelError("", "La descripción excede el límite de caracteres permitido.");
                }
                else
                {
                    ModelState.AddModelError("", "Error al guardar el requisito. Por favor, intente nuevamente.");
                }

                // Recargar información de la cláusula
                var clausula = await _context.Clausulas
                    .Include(c => c.NormaIso)
                    .FirstOrDefaultAsync(c => c.Id == modelo.ClausulaId);

                if (clausula != null)
                {
                    modelo.CodigoClausula = clausula.Codigo;
                    modelo.DescripcionClausula = clausula.Descripcion;
                    modelo.NombreNormaIso = $"{clausula.NormaIso.Nombre} - {clausula.NormaIso.Version}";
                    modelo.NormaIsoId = clausula.NormaIsoId;
                }

                return View(modelo);
            }
        }

        // GET: Requisitos/Detalles/5
        //[HttpGet]
        //[Authorize(Roles = $"{Constantes.RolAdministrador}, {Constantes.RolEncargadoSig}")]
        //public async Task<IActionResult> Detalles(int id)
        //{
        //    var requisito = await _context.Requisitos
        //        .Include(r => r.Clausula)
        //            .ThenInclude(c => c.NormaIso)
        //        .Include(r => r.Cumplimientos)
        //        .Include(r => r.Hallazgos)
        //        .FirstOrDefaultAsync(r => r.Id == id);

        //    if (requisito == null)
        //    {
        //        TempData["Error"] = "El requisito no existe";
        //        return RedirectToAction(nameof(Index));
        //    }

        //    var viewModel = new RequisitoDetalleViewModel
        //    {
        //        Id = requisito.Id,
        //        Descripcion = requisito.Descripcion,
        //        ClausulaId = requisito.ClausulaId,
        //        CodigoClausula = requisito.Clausula.Codigo,
        //        DescripcionClausula = requisito.Clausula.Descripcion,
        //        NombreNormaIso = $"{requisito.Clausula.NormaIso.Nombre} - {requisito.Clausula.NormaIso.Version}",
        //        NormaIsoId = requisito.Clausula.NormaIsoId,
        //        CantidadCumplimientos = requisito.Cumplimientos.Count,
        //        CantidadHallazgos = requisito.Hallazgos.Count
        //    };

        //    return View(viewModel);
        //}

        // GET: Requisitos/Editar/5
        [HttpGet]
        [Authorize(Roles = $"{Constantes.RolAdministrador}, {Constantes.RolEncargadoSig}")]
        public async Task<IActionResult> Editar(int id)
        {
            var requisito = await _context.Requisitos
                .Include(r => r.Clausula)
                    .ThenInclude(c => c.NormaIso)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (requisito == null)
            {
                TempData["Error"] = "El requisito no existe";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new RequisitoCrearViewModel
            {
                Id = requisito.Id,
                Descripcion = requisito.Descripcion,
                ClausulaId = requisito.ClausulaId,
                CodigoClausula = requisito.Clausula.Codigo,
                DescripcionClausula = requisito.Clausula.Descripcion,
                NombreNormaIso = $"{requisito.Clausula.NormaIso.Nombre} - {requisito.Clausula.NormaIso.Version}",
                NormaIsoId = requisito.Clausula.NormaIsoId
            };

            return View(viewModel);
        }

        // POST: Requisitos/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{Constantes.RolAdministrador}, {Constantes.RolEncargadoSig}")]
        public async Task<IActionResult> Editar(int id, RequisitoCrearViewModel modelo)
        {
            if (id != modelo.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                // Recargar información de la cláusula
                var clausulaError = await _context.Clausulas
                    .Include(c => c.NormaIso)
                    .FirstOrDefaultAsync(c => c.Id == modelo.ClausulaId);

                if (clausulaError != null)
                {
                    modelo.CodigoClausula = clausulaError.Codigo;
                    modelo.DescripcionClausula = clausulaError.Descripcion;
                    modelo.NombreNormaIso = $"{clausulaError.NormaIso.Nombre} - {clausulaError.NormaIso.Version}";
                    modelo.NormaIsoId = clausulaError.NormaIsoId;
                }

                return View(modelo);
            }

            try
            {
                var requisito = await _context.Requisitos.FindAsync(id);

                if (requisito == null)
                {
                    return NotFound();
                }

                requisito.Descripcion = modelo.Descripcion;

                _context.Update(requisito);
                await _context.SaveChangesAsync();

                TempData["Mensaje"] = "Requisito actualizado exitosamente";
                return RedirectToAction(nameof(Listado), new { id = requisito.ClausulaId });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RequisitoExists(modelo.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Error al actualizar el requisito. Por favor, intente nuevamente.");

                // Recargar información de la cláusula
                var clausula = await _context.Clausulas
                    .Include(c => c.NormaIso)
                    .FirstOrDefaultAsync(c => c.Id == modelo.ClausulaId);

                if (clausula != null)
                {
                    modelo.CodigoClausula = clausula.Codigo;
                    modelo.DescripcionClausula = clausula.Descripcion;
                    modelo.NombreNormaIso = $"{clausula.NormaIso.Nombre} - {clausula.NormaIso.Version}";
                    modelo.NormaIsoId = clausula.NormaIsoId;
                }

                return View(modelo);
            }
        }

        // POST: Requisitos/Eliminar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constantes.RolAdministrador)]
        public async Task<IActionResult> Eliminar(int id)
        {
            var requisito = await _context.Requisitos
                .Include(r => r.Cumplimientos)
                .Include(r => r.Hallazgos)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (requisito == null)
            {
                return NotFound();
            }

            try
            {
                // Verificar si tiene cumplimientos o hallazgos
                if (requisito.Cumplimientos.Any() || requisito.Hallazgos.Any())
                {
                    TempData["Error"] = $"No se puede eliminar el requisito porque tiene {requisito.Cumplimientos.Count} cumplimiento(s) y {requisito.Hallazgos.Count} hallazgo(s) registrados. Elimínelos primero.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Requisitos.Remove(requisito);
                await _context.SaveChangesAsync();

                TempData["Mensaje"] = "Requisito eliminado exitosamente";
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = "Error al eliminar el requisito. Por favor, intente nuevamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        // Método auxiliar
        private bool RequisitoExists(int id)
        {
            return _context.Requisitos.Any(e => e.Id == id);
        }
    }
}
