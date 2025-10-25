using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaGestionIso.Entidades;
using SistemaGestionIso.Models.Evidencia;
using SistemaGestionIso.Servicios;

namespace SistemaGestionIso.Controllers
{
    public class EvidenciasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Usuario> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EvidenciasController(
            ApplicationDbContext context,
            UserManager<Usuario> userManager,
            IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Evidencias (Todas las evidencias)
        [HttpGet]
        [Authorize(Roles = $"{Constantes.RolAdministrador}, {Constantes.RolEncargadoSig}, {Constantes.RolAuditor}")]
        public async Task<IActionResult> Index()
        {
            var evidencias = await _context.Evidencias
                .Include(e => e.Cumplimiento)
                    .ThenInclude(c => c.Requisito)
                        .ThenInclude(r => r.Clausula)
                            .ThenInclude(cl => cl.NormaIso)
                .Include(e => e.Usuario)
                .OrderByDescending(e => e.FechaSubida)
                .Select(e => new EvidenciaViewModel
                {
                    Id = e.Id,
                    CumplimientoId = e.CumplimientoId,
                    NombreArchivo = e.NombreArchivo,
                    RutaArchivo = e.RutaArchivo,
                    FechaSubida = e.FechaSubida,
                    UsuarioId = e.UsuarioId,
                    NombreUsuario = e.Usuario != null
                        ? $"{e.Usuario.PrimerNombre} {e.Usuario.PrimerApellido}"
                        : "Usuario desconocido",
                    EmailUsuario = e.Usuario != null ? e.Usuario.Email : "",
                    EstadoCumplimiento = e.Cumplimiento.Estado.ToString(),
                    FechaCumplimiento = e.Cumplimiento.Fecha,
                    DescripcionRequisito = e.Cumplimiento.Requisito.Descripcion,
                    CodigoClausula = e.Cumplimiento.Requisito.Clausula.Codigo,
                    NombreNormaIso = e.Cumplimiento.Requisito.Clausula.NormaIso.Nombre + " - " +
                                    e.Cumplimiento.Requisito.Clausula.NormaIso.Version
                })
                .ToListAsync();

            var viewModel = new EvidenciaListadoViewModel
            {
                Evidencias = evidencias,
                Mensaje = TempData["Mensaje"]?.ToString()
            };

            return View(viewModel);
        }

        // GET: Evidencias/Listado/5 (Evidencias de un cumplimiento específico)
        [HttpGet]
        [Authorize(Roles = $"{Constantes.RolAdministrador}, {Constantes.RolEncargadoSig}, {Constantes.RolAuditor}")]
        public async Task<IActionResult> Listado(int id)
        {
            var cumplimiento = await _context.Cumplimientos
                .Include(c => c.Requisito)
                    .ThenInclude(r => r.Clausula)
                        .ThenInclude(cl => cl.NormaIso)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cumplimiento == null)
            {
                TempData["Error"] = "El cumplimiento no existe";
                return RedirectToAction("Index", "Cumplimientos");
            }

            var evidencias = await _context.Evidencias
                .Where(e => e.CumplimientoId == id)
                .Include(e => e.Usuario)
                .OrderByDescending(e => e.FechaSubida)
                .Select(e => new EvidenciaViewModel
                {
                    Id = e.Id,
                    CumplimientoId = e.CumplimientoId,
                    NombreArchivo = e.NombreArchivo,
                    RutaArchivo = e.RutaArchivo,
                    FechaSubida = e.FechaSubida,
                    UsuarioId = e.UsuarioId,
                    NombreUsuario = e.Usuario != null
                        ? $"{e.Usuario.PrimerNombre} {e.Usuario.PrimerApellido}"
                        : "Usuario desconocido",
                    EmailUsuario = e.Usuario != null ? e.Usuario.Email : ""
                })
                .ToListAsync();

            var viewModel = new EvidenciaListadoViewModel
            {
                Evidencias = evidencias,
                Mensaje = TempData["Mensaje"]?.ToString(),
                CumplimientoId = id,
                EstadoCumplimiento = cumplimiento.Estado.ToString(),
                FechaCumplimiento = cumplimiento.Fecha
            };

            ViewBag.CumplimientoId = id;
            ViewBag.RequisitoId = cumplimiento.RequisitoId;
            ViewBag.DescripcionRequisito = cumplimiento.Requisito.Descripcion;
            ViewBag.CodigoClausula = cumplimiento.Requisito.Clausula.Codigo;
            ViewBag.NombreNormaIso = cumplimiento.Requisito.Clausula.NormaIso.Nombre;

            return View(viewModel);
        }

        // GET: Evidencias/Subir?cumplimientoId=5
        [HttpGet]
        [Authorize(Roles = $"{Constantes.RolAdministrador}, {Constantes.RolEncargadoSig}")]
        public async Task<IActionResult> Subir(int cumplimientoId)
        {
            var cumplimiento = await _context.Cumplimientos
                .Include(c => c.Requisito)
                    .ThenInclude(r => r.Clausula)
                        .ThenInclude(cl => cl.NormaIso)
                .FirstOrDefaultAsync(c => c.Id == cumplimientoId);

            if (cumplimiento == null)
            {
                TempData["Error"] = "El cumplimiento no existe";
                return RedirectToAction("Index", "Cumplimientos");
            }

            var viewModel = new EvidenciaSubirViewModel
            {
                CumplimientoId = cumplimientoId,
                DescripcionRequisito = cumplimiento.Requisito.Descripcion,
                CodigoClausula = cumplimiento.Requisito.Clausula.Codigo,
                NombreNormaIso = $"{cumplimiento.Requisito.Clausula.NormaIso.Nombre} - {cumplimiento.Requisito.Clausula.NormaIso.Version}",
                EstadoCumplimiento = cumplimiento.Estado.ToString()
            };

            return View(viewModel);
        }

        // POST: Evidencias/Subir
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{Constantes.RolAdministrador}, {Constantes.RolEncargadoSig}")]
        public async Task<IActionResult> Subir(EvidenciaSubirViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                await RecargarDatosCumplimiento(modelo);
                return View(modelo);
            }

            // Validar archivo
            if (modelo.Archivo == null || modelo.Archivo.Length == 0)
            {
                ModelState.AddModelError("Archivo", "Debe seleccionar un archivo");
                await RecargarDatosCumplimiento(modelo);
                return View(modelo);
            }

            // Validar tamaño (max 10 MB)
            if (modelo.Archivo.Length > 10 * 1024 * 1024)
            {
                ModelState.AddModelError("Archivo", "El archivo no puede superar los 10 MB");
                await RecargarDatosCumplimiento(modelo);
                return View(modelo);
            }

            // Validar extensión
            var extensionesPermitidas = new[] { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".jpg", ".jpeg", ".png", ".zip", ".rar" };
            var extension = Path.GetExtension(modelo.Archivo.FileName).ToLowerInvariant();

            if (!extensionesPermitidas.Contains(extension))
            {
                ModelState.AddModelError("Archivo", "Tipo de archivo no permitido. Extensiones permitidas: PDF, Word, Excel, Imágenes, ZIP, RAR");
                await RecargarDatosCumplimiento(modelo);
                return View(modelo);
            }

            try
            {
                // Obtener usuario actual
                var usuarioActual = await _userManager.GetUserAsync(User);

                // Crear carpeta si no existe
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "evidencias");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Generar nombre único para el archivo
                var nombreUnico = $"{Guid.NewGuid()}{extension}";
                var rutaCompleta = Path.Combine(uploadsFolder, nombreUnico);

                // Guardar archivo
                using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                {
                    await modelo.Archivo.CopyToAsync(stream);
                }

                // Crear registro en BD
                var evidencia = new Evidencia
                {
                    CumplimientoId = modelo.CumplimientoId,
                    NombreArchivo = modelo.Archivo.FileName,
                    RutaArchivo = $"/evidencias/{nombreUnico}",
                    FechaSubida = DateTime.Now,
                    UsuarioId = usuarioActual?.Id
                };

                _context.Evidencias.Add(evidencia);
                await _context.SaveChangesAsync();

                TempData["Mensaje"] = "Evidencia subida exitosamente";
                return RedirectToAction("Detalles", "Cumplimientos", new { id = modelo.CumplimientoId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al subir el archivo: {ex.Message}");
                await RecargarDatosCumplimiento(modelo);
                return View(modelo);
            }
        }

        // GET: Evidencias/Descargar/5
        [HttpGet]
        [Authorize(Roles = $"{Constantes.RolAdministrador}, {Constantes.RolEncargadoSig}, {Constantes.RolAuditor}")]
        public async Task<IActionResult> Descargar(int id)
        {
            var evidencia = await _context.Evidencias.FindAsync(id);

            if (evidencia == null)
            {
                TempData["Error"] = "La evidencia no existe";
                return RedirectToAction(nameof(Index));
            }

            var rutaCompleta = Path.Combine(_webHostEnvironment.WebRootPath, evidencia.RutaArchivo.TrimStart('/'));

            if (!System.IO.File.Exists(rutaCompleta))
            {
                TempData["Error"] = "El archivo no existe en el servidor";
                return RedirectToAction(nameof(Index));
            }

            var memory = new MemoryStream();
            using (var stream = new FileStream(rutaCompleta, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            var contentType = GetContentType(evidencia.NombreArchivo);
            return File(memory, contentType, evidencia.NombreArchivo);
        }

        // POST: Evidencias/Eliminar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{Constantes.RolAdministrador}, {Constantes.RolEncargadoSig}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var evidencia = await _context.Evidencias.FindAsync(id);

            if (evidencia == null)
            {
                return NotFound();
            }

            var cumplimientoId = evidencia.CumplimientoId;

            try
            {
                // Eliminar archivo físico
                var rutaCompleta = Path.Combine(_webHostEnvironment.WebRootPath, evidencia.RutaArchivo.TrimStart('/'));
                if (System.IO.File.Exists(rutaCompleta))
                {
                    System.IO.File.Delete(rutaCompleta);
                }

                // Eliminar registro de BD
                _context.Evidencias.Remove(evidencia);
                await _context.SaveChangesAsync();

                TempData["Mensaje"] = "Evidencia eliminada exitosamente";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al eliminar la evidencia: {ex.Message}";
            }

            return RedirectToAction("Detalles", "Cumplimientos", new { id = cumplimientoId });
        }

        // Métodos auxiliares
        private async Task RecargarDatosCumplimiento(EvidenciaSubirViewModel modelo)
        {
            var cumplimiento = await _context.Cumplimientos
                .Include(c => c.Requisito)
                    .ThenInclude(r => r.Clausula)
                        .ThenInclude(cl => cl.NormaIso)
                .FirstOrDefaultAsync(c => c.Id == modelo.CumplimientoId);

            if (cumplimiento != null)
            {
                modelo.DescripcionRequisito = cumplimiento.Requisito.Descripcion;
                modelo.CodigoClausula = cumplimiento.Requisito.Clausula.Codigo;
                modelo.NombreNormaIso = $"{cumplimiento.Requisito.Clausula.NormaIso.Nombre} - {cumplimiento.Requisito.Clausula.NormaIso.Version}";
                modelo.EstadoCumplimiento = cumplimiento.Estado.ToString();
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
    }
}
