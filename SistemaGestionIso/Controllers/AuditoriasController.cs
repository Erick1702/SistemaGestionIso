using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaGestionIso.Entidades;
using SistemaGestionIso.Models.Auditoria;
using SistemaGestionIso.Servicios;

namespace SistemaGestionIso.Controllers
{
    public class AuditoriasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Usuario> _userManager;

        public AuditoriasController(ApplicationDbContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Auditorias
        [HttpGet]
        [Authorize(Roles = $"{Constantes.RolAdministrador}, {Constantes.RolEncargadoSig}, {Constantes.RolAuditor}")]
        public async Task<IActionResult> Index()
        {
            var auditorias = await _context.Auditorias
                .Include(a => a.Usuario)
                .Include(a => a.Hallazgos)
                .OrderByDescending(a => a.FechaIni)
                .Select(a => new AuditoriaViewModel
                {
                    Id = a.Id,
                    FechaIni = a.FechaIni,
                    FechaFin = a.FechaFin,
                    AreaAuditada = a.AreaAuditada,
                    TipoAuditoria = a.TipoAuditoria,
                    UsuarioId = a.UsuarioId,
                    NombreAuditor = a.Usuario != null
                        ? $"{a.Usuario.PrimerNombre} {a.Usuario.PrimerApellido}"
                        : "No asignado",
                    EmailAuditor = a.Usuario != null ? a.Usuario.Email : "",
                    CantidadHallazgos = a.Hallazgos.Count,
                    CantidadConformidades = a.Hallazgos.Count(h => h.Tipo == "Conformidad"),
                    CantidadNoConformidades = a.Hallazgos.Count(h => h.Tipo == "No Conformidad"),
                    CantidadObservaciones = a.Hallazgos.Count(h => h.Tipo == "Observación")
                })
                .ToListAsync();

            var viewModel = new AuditoriaListadoViewModel
            {
                Auditorias = auditorias,
                Mensaje = TempData["Mensaje"]?.ToString()
            };

            return View(viewModel);
        }

        // GET: Auditorias/Crear
        [HttpGet]
        [Authorize(Roles = $"{Constantes.RolAdministrador}, {Constantes.RolEncargadoSig}")]
        public async Task<IActionResult> Crear()
        {
            // Obtener usuario actual como auditor por defecto
            var usuarioActual = await _userManager.GetUserAsync(User);

            var viewModel = new AuditoriaCrearEditarViewModel
            {
                FechaIni = DateTime.Now,
                FechaFin = DateTime.Now.AddDays(7),
                UsuarioId = usuarioActual?.Id,
                TipoAuditoria = TiposAuditoria.Interna
            };

            await CargarUsuarios();
            return View(viewModel);
        }

        // POST: Auditorias/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{Constantes.RolAdministrador}, {Constantes.RolEncargadoSig}")]
        public async Task<IActionResult> Crear(AuditoriaCrearEditarViewModel modelo)
        {
            // Validación personalizada de fechas
            if (modelo.FechaFin < modelo.FechaIni)
            {
                ModelState.AddModelError("FechaFin", "La fecha de fin no puede ser anterior a la fecha de inicio");
            }

            if (!ModelState.IsValid)
            {
                await CargarUsuarios();
                return View(modelo);
            }

            try
            {
                var auditoria = new Auditoria
                {
                    FechaIni = modelo.FechaIni,
                    FechaFin = modelo.FechaFin,
                    AreaAuditada = modelo.AreaAuditada,
                    TipoAuditoria = modelo.TipoAuditoria,
                    UsuarioId = modelo.UsuarioId
                };

                _context.Auditorias.Add(auditoria);
                await _context.SaveChangesAsync();

                TempData["Mensaje"] = "Auditoría creada exitosamente";
                return RedirectToAction(nameof(Detalles), new { id = auditoria.Id });
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "Error al guardar la auditoría. Por favor, intente nuevamente.");
                await CargarUsuarios();
                return View(modelo);
            }
        }

        // GET: Auditorias/Editar/5
        [HttpGet]
        [Authorize(Roles = $"{Constantes.RolAdministrador}, {Constantes.RolEncargadoSig}")]
        public async Task<IActionResult> Editar(int id)
        {
            var auditoria = await _context.Auditorias
                .Include(a => a.Usuario)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (auditoria == null)
            {
                TempData["Error"] = "La auditoría no existe";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new AuditoriaCrearEditarViewModel
            {
                Id = auditoria.Id,
                FechaIni = auditoria.FechaIni,
                FechaFin = auditoria.FechaFin,
                AreaAuditada = auditoria.AreaAuditada,
                TipoAuditoria = auditoria.TipoAuditoria,
                UsuarioId = auditoria.UsuarioId,
                NombreAuditor = auditoria.Usuario != null
                    ? $"{auditoria.Usuario.PrimerNombre} {auditoria.Usuario.PrimerApellido}"
                    : "No asignado"
            };

            await CargarUsuarios();
            return View(viewModel);
        }

        // POST: Auditorias/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{Constantes.RolAdministrador}, {Constantes.RolEncargadoSig}")]
        public async Task<IActionResult> Editar(int id, AuditoriaCrearEditarViewModel modelo)
        {
            if (id != modelo.Id)
            {
                return NotFound();
            }

            // Validación personalizada de fechas
            if (modelo.FechaFin < modelo.FechaIni)
            {
                ModelState.AddModelError("FechaFin", "La fecha de fin no puede ser anterior a la fecha de inicio");
            }

            if (!ModelState.IsValid)
            {
                await CargarUsuarios();
                return View(modelo);
            }

            try
            {
                var auditoria = await _context.Auditorias.FindAsync(id);

                if (auditoria == null)
                {
                    return NotFound();
                }

                auditoria.FechaIni = modelo.FechaIni;
                auditoria.FechaFin = modelo.FechaFin;
                auditoria.AreaAuditada = modelo.AreaAuditada;
                auditoria.TipoAuditoria = modelo.TipoAuditoria;
                auditoria.UsuarioId = modelo.UsuarioId;

                _context.Update(auditoria);
                await _context.SaveChangesAsync();

                TempData["Mensaje"] = "Auditoría actualizada exitosamente";
                return RedirectToAction(nameof(Detalles), new { id = auditoria.Id });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuditoriaExists(modelo.Id))
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
                ModelState.AddModelError("", "Error al actualizar la auditoría. Por favor, intente nuevamente.");
                await CargarUsuarios();
                return View(modelo);
            }
        }

        // GET: Auditorias/Detalles/5
        [HttpGet]
        [Authorize(Roles = $"{Constantes.RolAdministrador}, {Constantes.RolEncargadoSig}, {Constantes.RolAuditor}")]
        public async Task<IActionResult> Detalles(int id)
        {
            var auditoria = await _context.Auditorias
                .Include(a => a.Usuario)
                .Include(a => a.Hallazgos)
                    .ThenInclude(h => h.Requisito)
                        .ThenInclude(r => r.Clausula)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (auditoria == null)
            {
                TempData["Error"] = "La auditoría no existe";
                return RedirectToAction(nameof(Index));
            }

            var estadoAuditoria = DateTime.Now < auditoria.FechaIni ? "Programada" :
                                  DateTime.Now > auditoria.FechaFin ? "Finalizada" : "En Progreso";

            var colorEstado = estadoAuditoria switch
            {
                "Programada" => "info",
                "En Progreso" => "warning",
                "Finalizada" => "secondary",
                _ => "secondary"
            };

            var viewModel = new AuditoriaDetalleViewModel
            {
                Id = auditoria.Id,
                FechaIni = auditoria.FechaIni,
                FechaFin = auditoria.FechaFin,
                AreaAuditada = auditoria.AreaAuditada,
                TipoAuditoria = auditoria.TipoAuditoria,
                UsuarioId = auditoria.UsuarioId,
                NombreAuditor = auditoria.Usuario != null
                    ? $"{auditoria.Usuario.PrimerNombre} {auditoria.Usuario.PrimerApellido}"
                    : "No asignado",
                EmailAuditor = auditoria.Usuario?.Email,
                EstadoAuditoria = estadoAuditoria,
                ColorEstado = colorEstado,
                DuracionDias = (auditoria.FechaFin - auditoria.FechaIni).Days + 1,
                Hallazgos = auditoria.Hallazgos.Select(h => new HallazgoSimpleViewModel
                {
                    Id = h.Id,
                    Tipo = h.Tipo,
                    Descripcion = h.Descripcion,
                    DescripcionRequisito = h.Requisito?.Descripcion,
                    CodigoClausula = h.Requisito?.Clausula?.Codigo,
                    TieneEvidencia = !string.IsNullOrEmpty(h.EvidenciaPath)
                }).ToList(),
                CantidadHallazgos = auditoria.Hallazgos.Count,
                CantidadConformidades = auditoria.Hallazgos.Count(h => h.Tipo == "Conformidad"),
                CantidadNoConformidades = auditoria.Hallazgos.Count(h => h.Tipo == "No Conformidad"),
                CantidadObservaciones = auditoria.Hallazgos.Count(h => h.Tipo == "Observación")
            };

            return View(viewModel);
        }

        // POST: Auditorias/Eliminar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{Constantes.RolAdministrador}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var auditoria = await _context.Auditorias
                .Include(a => a.Hallazgos)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (auditoria == null)
            {
                return NotFound();
            }

            try
            {
                // Verificar si tiene hallazgos
                if (auditoria.Hallazgos.Any())
                {
                    TempData["Error"] = "No se puede eliminar la auditoría porque tiene hallazgos registrados. Elimine primero los hallazgos.";
                    return RedirectToAction(nameof(Detalles), new { id });
                }

                _context.Auditorias.Remove(auditoria);
                await _context.SaveChangesAsync();

                TempData["Mensaje"] = "Auditoría eliminada exitosamente";
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = "No se puede eliminar la auditoría porque tiene registros relacionados.";
                return RedirectToAction(nameof(Detalles), new { id });
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Auditorias/Estadisticas/5
        [HttpGet]
        [Authorize(Roles = $"{Constantes.RolAdministrador}, {Constantes.RolEncargadoSig}, {Constantes.RolAuditor}")]
        public async Task<IActionResult> Estadisticas(int id)
        {
            var auditoria = await _context.Auditorias
                .Include(a => a.Hallazgos)
                    .ThenInclude(h => h.Requisito)
                        .ThenInclude(r => r.Clausula)
                            .ThenInclude(c => c.NormaIso)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (auditoria == null)
            {
                TempData["Error"] = "La auditoría no existe";
                return RedirectToAction(nameof(Index));
            }

            // Aquí puedes agregar lógica para generar reportes estadísticos
            // Por ahora redirigimos a detalles
            return RedirectToAction(nameof(Detalles), new { id });
        }

        // Métodos auxiliares
        private async Task CargarUsuarios()
        {
            var usuarios = await _context.Users
                //.Where(u => u.Activo)
                .OrderBy(u => u.PrimerNombre)
                .ThenBy(u => u.PrimerApellido)
                .Select(u => new
                {
                    u.Id,
                    NombreCompleto = u.PrimerNombre + " " + u.PrimerApellido + " - " + u.Email
                })
                .ToListAsync();

            ViewBag.Usuarios = new SelectList(usuarios, "Id", "NombreCompleto");
        }

        private bool AuditoriaExists(int id)
        {
            return _context.Auditorias.Any(e => e.Id == id);
        }

        // API para obtener auditorías por fechas (útil para reportes)
        [HttpGet]
        [Authorize(Roles = $"{Constantes.RolAdministrador}, {Constantes.RolEncargadoSig}")]
        public async Task<IActionResult> ObtenerPorRangoFechas(DateTime fechaInicio, DateTime fechaFin)
        {
            var auditorias = await _context.Auditorias
                .Where(a => a.FechaIni >= fechaInicio && a.FechaFin <= fechaFin)
                .Include(a => a.Usuario)
                .Include(a => a.Hallazgos)
                .Select(a => new
                {
                    a.Id,
                    a.FechaIni,
                    a.FechaFin,
                    a.AreaAuditada,
                    a.TipoAuditoria,
                    Auditor = a.Usuario != null ? a.Usuario.PrimerNombre + " " + a.Usuario.PrimerApellido : "No asignado",
                    CantidadHallazgos = a.Hallazgos.Count
                })
                .ToListAsync();

            return Json(auditorias);
        }

        // API para obtener resumen de auditorías
        [HttpGet]
        [Authorize(Roles = $"{Constantes.RolAdministrador}, {Constantes.RolEncargadoSig}")]
        public async Task<IActionResult> ObtenerResumen()
        {
            var totalAuditorias = await _context.Auditorias.CountAsync();
            var programadas = await _context.Auditorias.CountAsync(a => a.FechaIni > DateTime.Now);
            var enProgreso = await _context.Auditorias
                .CountAsync(a => a.FechaIni <= DateTime.Now && a.FechaFin >= DateTime.Now);
            var finalizadas = await _context.Auditorias.CountAsync(a => a.FechaFin < DateTime.Now);
            var totalHallazgos = await _context.Hallazgos.CountAsync();

            var resumen = new
            {
                TotalAuditorias = totalAuditorias,
                Programadas = programadas,
                EnProgreso = enProgreso,
                Finalizadas = finalizadas,
                TotalHallazgos = totalHallazgos
            };

            return Json(resumen);
        }
    }
}
