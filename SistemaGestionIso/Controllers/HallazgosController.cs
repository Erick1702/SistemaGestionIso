using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaGestionIso.Entidades;
using SistemaGestionIso.Models.Hallazgo;
using SistemaGestionIso.Servicios;

namespace SistemaGestionIso.Controllers
{
    public class HallazgosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HallazgosController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Hallazgos (Todos los hallazgos)
        [HttpGet]
        [Authorize(Roles = $"{Constantes.RolAdministrador}, {Constantes.RolEncargadoSig}, {Constantes.RolAuditor}")]
        public async Task<IActionResult> Index()
        {
            var hallazgos = await _context.Hallazgos
                .Include(h => h.Auditoria)
                .Include(h => h.Requisito)
                    .ThenInclude(r => r.Clausula)
                        .ThenInclude(c => c.NormaIso)
                .OrderByDescending(h => h.Auditoria.FechaIni)
                .Select(h => new HallazgoViewModel
                {
                    Id = h.Id,
                    AuditoriaId = h.AuditoriaId,
                    AreaAuditada = h.Auditoria.AreaAuditada,
                    TipoAuditoria = h.Auditoria.TipoAuditoria,
                    FechaAuditoria = h.Auditoria.FechaIni,
                    RequisitoId = h.RequisitoId,
                    DescripcionRequisito = h.Requisito.Descripcion,
                    CodigoClausula = h.Requisito.Clausula.Codigo,
                    NombreNormaIso = h.Requisito.Clausula.NormaIso.Nombre + " - " + h.Requisito.Clausula.NormaIso.Version,
                    Tipo = h.Tipo,
                    Descripcion = h.Descripcion,
                    EvidenciaPath = h.EvidenciaPath
                })
                .ToListAsync();

            var viewModel = new HallazgoListadoViewModel
            {
                Hallazgos = hallazgos,
                Mensaje = TempData["Mensaje"]?.ToString()
            };

            return View(viewModel);
        }

        // GET: Hallazgos/Listado/5 (Hallazgos de una auditoría específica)
        [HttpGet]
        [Authorize(Roles = $"{Constantes.RolAdministrador}, {Constantes.RolEncargadoSig}, {Constantes.RolAuditor}")]
        public async Task<IActionResult> Listado(int id)
        {
            var auditoria = await _context.Auditorias
                .FirstOrDefaultAsync(a => a.Id == id);

            if (auditoria == null)
            {
                TempData["Error"] = "La auditoría no existe";
                return RedirectToAction("Index", "Auditorias");
            }

            var hallazgos = await _context.Hallazgos
                .Where(h => h.AuditoriaId == id)
                .Include(h => h.Requisito)
                    .ThenInclude(r => r.Clausula)
                        .ThenInclude(c => c.NormaIso)
                .Select(h => new HallazgoViewModel
                {
                    Id = h.Id,
                    AuditoriaId = h.AuditoriaId,
                    RequisitoId = h.RequisitoId,
                    DescripcionRequisito = h.Requisito.Descripcion,
                    CodigoClausula = h.Requisito.Clausula.Codigo,
                    NombreNormaIso = h.Requisito.Clausula.NormaIso.Nombre + " - " + h.Requisito.Clausula.NormaIso.Version,
                    Tipo = h.Tipo,
                    Descripcion = h.Descripcion,
                    EvidenciaPath = h.EvidenciaPath
                })
                .ToListAsync();

            var viewModel = new HallazgoListadoViewModel
            {
                Hallazgos = hallazgos,
                Mensaje = TempData["Mensaje"]?.ToString(),
                AuditoriaId = id,
                AreaAuditada = auditoria.AreaAuditada,
                TipoAuditoria = auditoria.TipoAuditoria
            };

            ViewBag.AuditoriaId = id;
            ViewBag.AreaAuditada = auditoria.AreaAuditada;
            ViewBag.TipoAuditoria = auditoria.TipoAuditoria;
            ViewBag.FechaInicioAuditoria = auditoria.FechaIni;
            ViewBag.FechaFinAuditoria = auditoria.FechaFin;

            return View(viewModel);
        }

        // GET: Hallazgos/Crear?auditoriaId=5
        [HttpGet]
        [Authorize(Roles = $"{Constantes.RolAdministrador}, {Constantes.RolEncargadoSig}, {Constantes.RolAuditor}")]
        public async Task<IActionResult> Crear(int auditoriaId)
        {
            var auditoria = await _context.Auditorias
                .FirstOrDefaultAsync(a => a.Id == auditoriaId);

            if (auditoria == null)
            {
                TempData["Error"] = "La auditoría no existe";
                return RedirectToAction("Index", "Auditorias");
            }

            var viewModel = new HallazgoCrearEditarViewModel
            {
                AuditoriaId = auditoriaId,
                AreaAuditada = auditoria.AreaAuditada,
                TipoAuditoria = auditoria.TipoAuditoria,
                Tipo = TiposHallazgo.Conformidad
            };

            await CargarRequisitos();
            return View(viewModel);
        }

        // POST: Hallazgos/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{Constantes.RolAdministrador}, {Constantes.RolEncargadoSig}, {Constantes.RolAuditor}")]
        public async Task<IActionResult> Crear(HallazgoCrearEditarViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                await RecargarDatosAuditoria(modelo);
                await CargarRequisitos();
                return View(modelo);
            }

            try
            {
                var hallazgo = new Hallazgo
                {
                    AuditoriaId = modelo.AuditoriaId,
                    RequisitoId = modelo.RequisitoId,
                    Tipo = modelo.Tipo,
                    Descripcion = modelo.Descripcion
                };

                // Procesar archivo de evidencia si existe
                if (modelo.EvidenciaArchivo != null && modelo.EvidenciaArchivo.Length > 0)
                {
                    var evidenciaPath = await GuardarEvidencia(modelo.EvidenciaArchivo);
                    if (evidenciaPath != null)
                    {
                        hallazgo.EvidenciaPath = evidenciaPath;
                    }
                }

                _context.Hallazgos.Add(hallazgo);
                await _context.SaveChangesAsync();

                TempData["Mensaje"] = "Hallazgo registrado exitosamente";
                return RedirectToAction(nameof(Listado), new { id = modelo.AuditoriaId });
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "Error al guardar el hallazgo. Por favor, intente nuevamente.");
                await RecargarDatosAuditoria(modelo);
                await CargarRequisitos();
                return View(modelo);
            }
        }

        // GET: Hallazgos/Editar/5
        [HttpGet]
        [Authorize(Roles = $"{Constantes.RolAdministrador}, {Constantes.RolEncargadoSig}, {Constantes.RolAuditor}")]
        public async Task<IActionResult> Editar(int id)
        {
            var hallazgo = await _context.Hallazgos
                .Include(h => h.Auditoria)
                .Include(h => h.Requisito)
                    .ThenInclude(r => r.Clausula)
                        .ThenInclude(c => c.NormaIso)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hallazgo == null)
            {
                TempData["Error"] = "El hallazgo no existe";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new HallazgoCrearEditarViewModel
            {
                Id = hallazgo.Id,
                AuditoriaId = hallazgo.AuditoriaId,
                AreaAuditada = hallazgo.Auditoria.AreaAuditada,
                TipoAuditoria = hallazgo.Auditoria.TipoAuditoria,
                RequisitoId = hallazgo.RequisitoId,
                DescripcionRequisito = hallazgo.Requisito.Descripcion,
                CodigoClausula = hallazgo.Requisito.Clausula.Codigo,
                NombreNormaIso = $"{hallazgo.Requisito.Clausula.NormaIso.Nombre} - {hallazgo.Requisito.Clausula.NormaIso.Version}",
                Tipo = hallazgo.Tipo,
                Descripcion = hallazgo.Descripcion,
                EvidenciaPath = hallazgo.EvidenciaPath
            };

            await CargarRequisitos();
            return View(viewModel);
        }

        // POST: Hallazgos/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{Constantes.RolAdministrador}, {Constantes.RolEncargadoSig}, {Constantes.RolAuditor}")]
        public async Task<IActionResult> Editar(int id, HallazgoCrearEditarViewModel modelo)
        {
            if (id != modelo.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                await RecargarDatosAuditoria(modelo);
                await CargarRequisitos();
                return View(modelo);
            }

            try
            {
                var hallazgo = await _context.Hallazgos.FindAsync(id);

                if (hallazgo == null)
                {
                    return NotFound();
                }

                hallazgo.RequisitoId = modelo.RequisitoId;
                hallazgo.Tipo = modelo.Tipo;
                hallazgo.Descripcion = modelo.Descripcion;

                // Procesar nueva evidencia si se subió
                if (modelo.EvidenciaArchivo != null && modelo.EvidenciaArchivo.Length > 0)
                {
                    // Eliminar evidencia anterior si existe
                    if (!string.IsNullOrEmpty(hallazgo.EvidenciaPath))
                    {
                        EliminarEvidencia(hallazgo.EvidenciaPath);
                    }

                    var evidenciaPath = await GuardarEvidencia(modelo.EvidenciaArchivo);
                    if (evidenciaPath != null)
                    {
                        hallazgo.EvidenciaPath = evidenciaPath;
                    }
                }

                _context.Update(hallazgo);
                await _context.SaveChangesAsync();

                TempData["Mensaje"] = "Hallazgo actualizado exitosamente";
                return RedirectToAction(nameof(Listado), new { id = hallazgo.AuditoriaId });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HallazgoExists(modelo.Id))
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
                ModelState.AddModelError("", "Error al actualizar el hallazgo. Por favor, intente nuevamente.");
                await RecargarDatosAuditoria(modelo);
                await CargarRequisitos();
                return View(modelo);
            }
        }

        // POST: Hallazgos/Eliminar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{Constantes.RolAdministrador}, {Constantes.RolEncargadoSig}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var hallazgo = await _context.Hallazgos.FindAsync(id);

            if (hallazgo == null)
            {
                return NotFound();
            }

            var auditoriaId = hallazgo.AuditoriaId;

            try
            {
                // Eliminar evidencia física si existe
                if (!string.IsNullOrEmpty(hallazgo.EvidenciaPath))
                {
                    EliminarEvidencia(hallazgo.EvidenciaPath);
                }

                _context.Hallazgos.Remove(hallazgo);
                await _context.SaveChangesAsync();

                TempData["Mensaje"] = "Hallazgo eliminado exitosamente";
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = "Error al eliminar el hallazgo. Por favor, intente nuevamente.";
            }

            return RedirectToAction(nameof(Listado), new { id = auditoriaId });
        }

        // GET: Hallazgos/Detalles/5
        [HttpGet]
        [Authorize(Roles = $"{Constantes.RolAdministrador}, {Constantes.RolEncargadoSig}, {Constantes.RolAuditor}")]
        public async Task<IActionResult> Detalles(int id)
        {
            var hallazgo = await _context.Hallazgos
                .Include(h => h.Auditoria)
                    .ThenInclude(a => a.Usuario)
                .Include(h => h.Requisito)
                    .ThenInclude(r => r.Clausula)
                        .ThenInclude(c => c.NormaIso)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hallazgo == null)
            {
                TempData["Error"] = "El hallazgo no existe";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new HallazgoDetalleViewModel
            {
                Id = hallazgo.Id,
                AuditoriaId = hallazgo.AuditoriaId,
                AreaAuditada = hallazgo.Auditoria.AreaAuditada,
                TipoAuditoria = hallazgo.Auditoria.TipoAuditoria,
                FechaInicioAuditoria = hallazgo.Auditoria.FechaIni,
                FechaFinAuditoria = hallazgo.Auditoria.FechaFin,
                RequisitoId = hallazgo.RequisitoId,
                DescripcionRequisito = hallazgo.Requisito.Descripcion ?? "",
                CodigoClausula = hallazgo.Requisito.Clausula.Codigo,
                DescripcionClausula = hallazgo.Requisito.Clausula.Descripcion,
                NombreNormaIso = $"{hallazgo.Requisito.Clausula.NormaIso.Nombre} - {hallazgo.Requisito.Clausula.NormaIso.Version}",
                Tipo = hallazgo.Tipo,
                TipoColor = hallazgo.Tipo switch
                {
                    "Conformidad" => "success",
                    "No Conformidad" => "danger",
                    "Observación" => "warning",
                    _ => "secondary"
                },
                Descripcion = hallazgo.Descripcion,
                EvidenciaPath = hallazgo.EvidenciaPath,
                NombreArchivoEvidencia = !string.IsNullOrEmpty(hallazgo.EvidenciaPath)
                    ? Path.GetFileName(hallazgo.EvidenciaPath)
                    : null,
                NombreAuditor = hallazgo.Auditoria.Usuario != null
                    ? $"{hallazgo.Auditoria.Usuario.PrimerNombre} {hallazgo.Auditoria.Usuario.PrimerApellido}"
                    : "No asignado",
                EmailAuditor = hallazgo.Auditoria.Usuario?.Email
            };

            return View(viewModel);
        }

        // GET: Hallazgos/DescargarEvidencia/5
        [HttpGet]
        [Authorize(Roles = $"{Constantes.RolAdministrador}, {Constantes.RolEncargadoSig}, {Constantes.RolAuditor}")]
        public async Task<IActionResult> DescargarEvidencia(int id)
        {
            var hallazgo = await _context.Hallazgos.FindAsync(id);

            if (hallazgo == null || string.IsNullOrEmpty(hallazgo.EvidenciaPath))
            {
                TempData["Error"] = "No hay evidencia disponible";
                return RedirectToAction(nameof(Detalles), new { id });
            }

            var rutaCompleta = Path.Combine(_webHostEnvironment.WebRootPath, hallazgo.EvidenciaPath.TrimStart('/'));

            if (!System.IO.File.Exists(rutaCompleta))
            {
                TempData["Error"] = "El archivo de evidencia no existe en el servidor";
                return RedirectToAction(nameof(Detalles), new { id });
            }

            var memory = new MemoryStream();
            using (var stream = new FileStream(rutaCompleta, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            var nombreArchivo = Path.GetFileName(hallazgo.EvidenciaPath);
            var contentType = GetContentType(nombreArchivo);

            return File(memory, contentType, nombreArchivo);
        }

        // Métodos auxiliares
        private async Task CargarRequisitos()
        {
            var requisitos = await _context.Requisitos
                .Include(r => r.Clausula)
                    .ThenInclude(c => c.NormaIso)
                .OrderBy(r => r.Clausula.NormaIso.Nombre)
                    .ThenBy(r => r.Clausula.Orden)
                .Select(r => new RequisitoDropdownViewModel
                {
                    Id = r.Id,
                    Descripcion = r.Descripcion ?? "Sin descripción",
                    CodigoClausula = r.Clausula.Codigo,
                    NombreNormaIso = r.Clausula.NormaIso.Nombre
                })
                .ToListAsync();

            ViewBag.Requisitos = new SelectList(requisitos, "Id", "TextoCompleto");
        }

        private async Task RecargarDatosAuditoria(HallazgoCrearEditarViewModel modelo)
        {
            var auditoria = await _context.Auditorias
                .FirstOrDefaultAsync(a => a.Id == modelo.AuditoriaId);

            if (auditoria != null)
            {
                modelo.AreaAuditada = auditoria.AreaAuditada;
                modelo.TipoAuditoria = auditoria.TipoAuditoria;
            }

            if (modelo.RequisitoId > 0)
            {
                var requisito = await _context.Requisitos
                    .Include(r => r.Clausula)
                        .ThenInclude(c => c.NormaIso)
                    .FirstOrDefaultAsync(r => r.Id == modelo.RequisitoId);

                if (requisito != null)
                {
                    modelo.DescripcionRequisito = requisito.Descripcion;
                    modelo.CodigoClausula = requisito.Clausula.Codigo;
                    modelo.NombreNormaIso = $"{requisito.Clausula.NormaIso.Nombre} - {requisito.Clausula.NormaIso.Version}";
                }
            }
        }

        private async Task<string?> GuardarEvidencia(IFormFile archivo)
        {
            try
            {
                // Validar tamaño (max 10 MB)
                if (archivo.Length > 10 * 1024 * 1024)
                {
                    return null;
                }

                // Validar extensión
                var extensionesPermitidas = new[] { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".jpg", ".jpeg", ".png", ".zip", ".rar" };
                var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();

                if (!extensionesPermitidas.Contains(extension))
                {
                    return null;
                }

                // Crear carpeta si no existe
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "hallazgos");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Generar nombre único
                var nombreUnico = $"{Guid.NewGuid()}{extension}";
                var rutaCompleta = Path.Combine(uploadsFolder, nombreUnico);

                // Guardar archivo
                using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                {
                    await archivo.CopyToAsync(stream);
                }

                return $"/hallazgos/{nombreUnico}";
            }
            catch
            {
                return null;
            }
        }

        private void EliminarEvidencia(string evidenciaPath)
        {
            try
            {
                var rutaCompleta = Path.Combine(_webHostEnvironment.WebRootPath, evidenciaPath.TrimStart('/'));
                if (System.IO.File.Exists(rutaCompleta))
                {
                    System.IO.File.Delete(rutaCompleta);
                }
            }
            catch
            {
                // Log error pero no fallar la operación
            }
        }

        private string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".zip" => "application/zip",
                ".rar" => "application/x-rar-compressed",
                _ => "application/octet-stream"
            };
        }

        private bool HallazgoExists(int id)
        {
            return _context.Hallazgos.Any(e => e.Id == id);
        }
    }
}
