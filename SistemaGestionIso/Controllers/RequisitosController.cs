using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaGestionIso.Entidades;
using SistemaGestionIso.Models;
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
    }
}
