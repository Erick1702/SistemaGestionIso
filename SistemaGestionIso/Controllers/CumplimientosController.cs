using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaGestionIso.Entidades;
using SistemaGestionIso.Models.Cumplimiento;
using SistemaGestionIso.Servicios;

namespace SistemaGestionIso.Controllers
{
    public class CumplimientosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Usuario> _userManager;

        public CumplimientosController(ApplicationDbContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Cumplimientos (Todos los cumplimientos)
        [HttpGet]
        [Authorize(Roles = $"{Constantes.RolAdministrador}, {Constantes.RolEncargadoSig}, {Constantes.RolAuditor}")]
        public async Task<IActionResult> Index()
        {
            var cumplimientos = await _context.Cumplimientos
                .Include(c => c.Requisito)
                    .ThenInclude(r => r.Clausula)
                        .ThenInclude(cl => cl.NormaIso)
                .Include(c => c.Evidencias)
                .Include(c => c.UsuarioResponsable)
                .OrderByDescending(c => c.Fecha)
                .Select(c => new CumplimientoViewModel
                {
                    Id = c.Id,
                    RequisitoId = c.RequisitoId,
                    DescripcionRequisito = c.Requisito.Descripcion,
                    CodigoClausula = c.Requisito.Clausula.Codigo,
                    NombreNormaIso = c.Requisito.Clausula.NormaIso.Nombre + " - " + c.Requisito.Clausula.NormaIso.Version,
                    ClausulaId = c.Requisito.ClausulaId,
                    NormaIsoId = c.Requisito.Clausula.NormaIsoId,
                    Estado = c.Estado,
                    Fecha = c.Fecha,
                    Observaciones = c.Observaciones,
                    UsuarioResponsableId = c.UsuarioResponsableId,
                    NombreUsuarioResponsable = c.UsuarioResponsable != null
                        ? c.UsuarioResponsable.PrimerNombre + " " + c.UsuarioResponsable.PrimerApellido
                        : "No asignado",
                    CantidadEvidencias = c.Evidencias.Count
                })
                .ToListAsync();

            var viewModel = new CumplimientoListadoViewModel
            {
                Cumplimientos = cumplimientos,
                Mensaje = TempData["Mensaje"]?.ToString()
            };

            return View(viewModel);
        }


        // GET: Cumplimientos/Listado/5 (Cumplimientos de un requisito específico)
        [HttpGet]
        [Authorize(Roles = $"{Constantes.RolAdministrador}, {Constantes.RolEncargadoSig}, {Constantes.RolAuditor}")]
        public async Task<IActionResult> Listado(int id)
        {
            var requisito = await _context.Requisitos
                .Include(r => r.Clausula)
                    .ThenInclude(c => c.NormaIso)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (requisito == null)
            {
                TempData["Error"] = "El requisito no existe";
                return RedirectToAction("Index", "Requisitos");
            }

            var cumplimientos = await _context.Cumplimientos
                .Where(c => c.RequisitoId == id)
                .Include(c => c.Evidencias)
                .Include(c => c.UsuarioResponsable)
                .OrderByDescending(c => c.Fecha)
                .Select(c => new CumplimientoViewModel
                {
                    Id = c.Id,
                    RequisitoId = c.RequisitoId,
                    DescripcionRequisito = c.Requisito.Descripcion,
                    CodigoClausula = c.Requisito.Clausula.Codigo,
                    NombreNormaIso = c.Requisito.Clausula.NormaIso.Nombre + " - " + c.Requisito.Clausula.NormaIso.Version,
                    ClausulaId = c.Requisito.ClausulaId,
                    NormaIsoId = c.Requisito.Clausula.NormaIsoId,
                    Estado = c.Estado,
                    Fecha = c.Fecha,
                    Observaciones = c.Observaciones,
                    UsuarioResponsableId = c.UsuarioResponsableId,
                    NombreUsuarioResponsable = c.UsuarioResponsable != null
                        ? c.UsuarioResponsable.PrimerNombre + " " + c.UsuarioResponsable.PrimerApellido
                        : "No asignado",
                    CantidadEvidencias = c.Evidencias.Count
                })
                .ToListAsync();

            var viewModel = new CumplimientoListadoViewModel
            {
                Cumplimientos = cumplimientos,
                Mensaje = TempData["Mensaje"]?.ToString()
            };

            ViewBag.RequisitoId = id;
            ViewBag.DescripcionRequisito = requisito.Descripcion;
            ViewBag.CodigoClausula = requisito.Clausula.Codigo;
            ViewBag.DescripcionClausula = requisito.Clausula.Descripcion;
            ViewBag.NombreNormaIso = requisito.Clausula.NormaIso.Nombre;
            ViewBag.VersionNormaIso = requisito.Clausula.NormaIso.Version;
            ViewBag.ClausulaId = requisito.ClausulaId;
            ViewBag.NormaIsoId = requisito.Clausula.NormaIsoId;

            return View(viewModel);
        }

        // GET: Cumplimientos/Crear?requisitoId=5
        [HttpGet]
        [Authorize(Roles = $"{Constantes.RolAdministrador}, {Constantes.RolEncargadoSig}")]
        public async Task<IActionResult> Crear(int requisitoId)
        {
            var requisito = await _context.Requisitos
                .Include(r => r.Clausula)
                    .ThenInclude(c => c.NormaIso)
                .FirstOrDefaultAsync(r => r.Id == requisitoId);

            if (requisito == null)
            {
                TempData["Error"] = "El requisito no existe";
                return RedirectToAction("Index", "Requisitos");
            }

            // Obtener usuario actual
            var usuarioActual = await _userManager.GetUserAsync(User);

            var viewModel = new CumplimientoCrearEditarViewModel
            {
                RequisitoId = requisitoId,
                DescripcionRequisito = requisito.Descripcion,
                CodigoClausula = requisito.Clausula.Codigo,
                NombreNormaIso = $"{requisito.Clausula.NormaIso.Nombre} - {requisito.Clausula.NormaIso.Version}",
                ClausulaId = requisito.ClausulaId,
                NormaIsoId = requisito.Clausula.NormaIsoId,
                Fecha = DateTime.Now,
                Estado = EstadoCumplimiento.Cumple,
                UsuarioResponsableId = usuarioActual?.Id
            };

            // Cargar lista de usuarios para el dropdown
            await CargarUsuarios();

            return View(viewModel);
        }

        // POST: Cumplimientos/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{Constantes.RolAdministrador}, {Constantes.RolEncargadoSig}")]
        public async Task<IActionResult> Crear(CumplimientoCrearEditarViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                await RecargarDatosRequisito(modelo);
                await CargarUsuarios();
                return View(modelo);
            }

            try
            {
                var cumplimiento = new Cumplimiento
                {
                    RequisitoId = modelo.RequisitoId,
                    Estado = modelo.Estado,
                    Fecha = modelo.Fecha,
                    Observaciones = modelo.Observaciones,
                    UsuarioResponsableId = modelo.UsuarioResponsableId
                };

                _context.Cumplimientos.Add(cumplimiento);
                await _context.SaveChangesAsync();

                TempData["Mensaje"] = "Cumplimiento registrado exitosamente";
                return RedirectToAction(nameof(Listado), new { id = modelo.RequisitoId });
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "Error al guardar el cumplimiento. Por favor, intente nuevamente.");
                await RecargarDatosRequisito(modelo);
                await CargarUsuarios();
                return View(modelo);
            }
        }

        // GET: Cumplimientos/Editar/5
        [HttpGet]
        [Authorize(Roles = $"{Constantes.RolAdministrador}, {Constantes.RolEncargadoSig}")]
        public async Task<IActionResult> Editar(int id)
        {
            var cumplimiento = await _context.Cumplimientos
                .Include(c => c.Requisito)
                    .ThenInclude(r => r.Clausula)
                        .ThenInclude(cl => cl.NormaIso)
                .Include(c => c.UsuarioResponsable)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cumplimiento == null)
            {
                TempData["Error"] = "El cumplimiento no existe";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new CumplimientoCrearEditarViewModel
            {
                Id = cumplimiento.Id,
                RequisitoId = cumplimiento.RequisitoId,
                DescripcionRequisito = cumplimiento.Requisito.Descripcion,
                CodigoClausula = cumplimiento.Requisito.Clausula.Codigo,
                NombreNormaIso = $"{cumplimiento.Requisito.Clausula.NormaIso.Nombre} - {cumplimiento.Requisito.Clausula.NormaIso.Version}",
                ClausulaId = cumplimiento.Requisito.ClausulaId,
                NormaIsoId = cumplimiento.Requisito.Clausula.NormaIsoId,
                Estado = cumplimiento.Estado,
                Fecha = cumplimiento.Fecha,
                Observaciones = cumplimiento.Observaciones,
                UsuarioResponsableId = cumplimiento.UsuarioResponsableId
            };

            await CargarUsuarios();

            return View(viewModel);
        }

        // POST: Cumplimientos/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{Constantes.RolAdministrador}, {Constantes.RolEncargadoSig}")]
        public async Task<IActionResult> Editar(int id, CumplimientoCrearEditarViewModel modelo)
        {
            if (id != modelo.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                await RecargarDatosRequisito(modelo);
                await CargarUsuarios();
                return View(modelo);
            }

            try
            {
                var cumplimiento = await _context.Cumplimientos.FindAsync(id);

                if (cumplimiento == null)
                {
                    return NotFound();
                }

                cumplimiento.Estado = modelo.Estado;
                cumplimiento.Fecha = modelo.Fecha;
                cumplimiento.Observaciones = modelo.Observaciones;
                cumplimiento.UsuarioResponsableId = modelo.UsuarioResponsableId;

                _context.Update(cumplimiento);
                await _context.SaveChangesAsync();

                TempData["Mensaje"] = "Cumplimiento actualizado exitosamente";
                return RedirectToAction(nameof(Listado), new { id = cumplimiento.RequisitoId });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CumplimientoExists(modelo.Id))
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
                ModelState.AddModelError("", "Error al actualizar el cumplimiento. Por favor, intente nuevamente.");
                await RecargarDatosRequisito(modelo);
                await CargarUsuarios();
                return View(modelo);
            }
        }

        // POST: Cumplimientos/Eliminar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{Constantes.RolAdministrador}, {Constantes.RolEncargadoSig}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var cumplimiento = await _context.Cumplimientos.FindAsync(id);

            if (cumplimiento == null)
            {
                return NotFound();
            }

            var requisitoId = cumplimiento.RequisitoId;

            try
            {
                _context.Cumplimientos.Remove(cumplimiento);
                await _context.SaveChangesAsync();

                TempData["Mensaje"] = "Cumplimiento eliminado exitosamente";
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = "No se puede eliminar el cumplimiento porque tiene evidencias relacionadas.";
            }

            return RedirectToAction(nameof(Listado), new { id = requisitoId });
        }

        // GET: Cumplimientos/Detalles/5
        [HttpGet]
        [Authorize(Roles = $"{Constantes.RolAdministrador}, {Constantes.RolEncargadoSig}, {Constantes.RolAuditor}")]
        public async Task<IActionResult> Detalles(int id)
        {
            var cumplimiento = await _context.Cumplimientos
                .Include(c => c.Requisito)
                    .ThenInclude(r => r.Clausula)
                        .ThenInclude(cl => cl.NormaIso)
                .Include(c => c.Evidencias)
                    .ThenInclude(e => e.Usuario)
                .Include(c => c.UsuarioResponsable)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cumplimiento == null)
            {
                return NotFound();
            }

            var viewModel = new CumplimientoDetalleViewModel
            {
                Id = cumplimiento.Id,
                RequisitoId = cumplimiento.RequisitoId,
                DescripcionRequisito = cumplimiento.Requisito.Descripcion,
                CodigoClausula = cumplimiento.Requisito.Clausula.Codigo,
                DescripcionClausula = cumplimiento.Requisito.Clausula.Descripcion,
                NombreNormaIso = $"{cumplimiento.Requisito.Clausula.NormaIso.Nombre} - {cumplimiento.Requisito.Clausula.NormaIso.Version}",
                Estado = cumplimiento.Estado,
                EstadoTexto = cumplimiento.Estado switch
                {
                    EstadoCumplimiento.Cumple => "Cumple",
                    EstadoCumplimiento.Parcial => "Cumple Parcialmente",
                    EstadoCumplimiento.NoCumple => "No Cumple",
                    _ => "Desconocido"
                },
                EstadoColor = cumplimiento.Estado switch
                {
                    EstadoCumplimiento.Cumple => "success",
                    EstadoCumplimiento.Parcial => "warning",
                    EstadoCumplimiento.NoCumple => "danger",
                    _ => "secondary"
                },
                Fecha = cumplimiento.Fecha,
                Observaciones = cumplimiento.Observaciones,
                UsuarioResponsableId = cumplimiento.UsuarioResponsableId,
                NombreUsuarioResponsable = cumplimiento.UsuarioResponsable != null
                    ? $"{cumplimiento.UsuarioResponsable.PrimerNombre} {cumplimiento.UsuarioResponsable.PrimerApellido}"
                    : "No asignado",
                EmailUsuarioResponsable = cumplimiento.UsuarioResponsable?.Email,
                CantidadEvidencias = cumplimiento.Evidencias.Count,
                Evidencias = cumplimiento.Evidencias.Select(e => new EvidenciaSimpleViewModel
                {
                    Id = e.Id,
                    NombreArchivo = e.NombreArchivo,
                    RutaArchivo = e.RutaArchivo,
                    FechaSubida = e.FechaSubida,
                    NombreUsuario = e.Usuario != null
                        ? $"{e.Usuario.PrimerNombre} {e.Usuario.PrimerApellido}"
                        : "Usuario desconocido"
                }).ToList()
            };

            return View(viewModel);
        }


        // Agregar al final de CumplimientosController.cs

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

        private async Task RecargarDatosRequisito(CumplimientoCrearEditarViewModel modelo)
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
                modelo.ClausulaId = requisito.ClausulaId;
                modelo.NormaIsoId = requisito.Clausula.NormaIsoId;
            }
        }

        private bool CumplimientoExists(int id)
        {
            return _context.Cumplimientos.Any(e => e.Id == id);
        }


        

    }
}
