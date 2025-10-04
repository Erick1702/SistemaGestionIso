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

        [HttpGet]
        [Authorize(Roles = $"{Constantes.RolAdministrador}, {Constantes.RolEncargadoSig}")]
        public async Task<IActionResult> Listado (int Id, string? mensaje = null)
        {
            var clausula = await _context.Clausulas.FirstOrDefaultAsync(c => c.Id == Id);

            if (clausula == null)
            {
                TempData["Error"] = "La cláusula no existe";
                return RedirectToAction("Crear", "Clausulas");
            }

            var requisitos = await _context.Requisitos
                .Where(r => r.ClausulaId == Id)
                .Select(r => new RequisitoViewModel
                {
                    Id = r.Id,
                    Descripcion = r.Descripcion,
                    ClausulaId = r.ClausulaId,
                    CodigoClausula = r.Clausula.Codigo
                   
                })
                .ToListAsync();

            if (requisitos == null || requisitos.Count == 0)
            {
                // No hay requisitos, vamos a crear el primero
                return RedirectToAction("Crear", new { clausulaId = Id });
            }

            var viewModel = new RequisitoListadoViewModel
            {
                Requisitos = requisitos,
                Mensaje = TempData["Mensaje"]?.ToString()
            };
            ViewBag.ClausulaId = Id;
            ViewBag.CodigoClausula = clausula.Codigo;
            ViewBag.DescripcionClausula = clausula.Descripcion;
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Crear(int clausulaId)
        {
            var clausula = await _context.Clausulas.FirstOrDefaultAsync(c => c.Id == clausulaId);
            if (clausula == null)
            {
                TempData["Error"] = "La cláusula no existe";
                return RedirectToAction("Listado", "Clausulas");
            }
            var modelo = new RequisitoCrearViewModel
            {
                ClausulaId = clausulaId,
                CodigoClausula = clausula.Codigo
            };
            return View(modelo);
        }


        [HttpPost]
        [Authorize(Roles = $"{Constantes.RolAdministrador}, {Constantes.RolEncargadoSig}")]
        public async Task<IActionResult> Crear(RequisitoCrearViewModel modelo)
        {
            if(!ModelState.IsValid)
            {
                return View(modelo);
            }

            var requisito = new Requisito
            {
                Descripcion = modelo.Descripcion,
                ClausulaId = modelo.ClausulaId
            };


            _context.Add(requisito);
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "El requisito se creó correctamente.";
            return RedirectToAction("Listado", new { Id = modelo.ClausulaId } );

        }
    }
}
