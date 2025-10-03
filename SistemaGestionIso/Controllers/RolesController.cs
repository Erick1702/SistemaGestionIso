using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SistemaGestionIso.Entidades;
using SistemaGestionIso.Models;

namespace SistemaGestionIso.Controllers
{
    public class RolesController : Controller
    {
        private readonly RoleManager<Rol> _roleManager;

        public RolesController(RoleManager<Rol> roleManager)
        {
            _roleManager = roleManager;
        }

        // GET: Roles
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var roles = _roleManager.Roles
                .Select(r => new RolViewModel
                {
                    RolId = r.Id,
                    Nombre = r.Name!,
                    Descripcion = r.Descripcion
                }).ToList();

            return View(roles);
        }

        // GET: Roles/Create
        [AllowAnonymous]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Roles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Create(RolViewModel model)
        {
            if (ModelState.IsValid)
            {
                var rol = new Rol { Name = model.Nombre, Descripcion = model.Descripcion };
                var result = await _roleManager.CreateAsync(rol);

                if (result.Succeeded)
                    return RedirectToAction(nameof(Index));

                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }

        // GET: Roles/Edit/5
        [AllowAnonymous]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            var rol = await _roleManager.FindByIdAsync(id);
            if (rol == null) return NotFound();

            var model = new RolViewModel
            {
                RolId = rol.Id,
                Nombre = rol.Name!,
                Descripcion = rol.Descripcion
            };

            return View(model);
        }

        // POST: Roles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Edit(string id, RolViewModel model)
        {
            if (id != model.RolId) return NotFound();

            if (ModelState.IsValid)
            {
                var rol = await _roleManager.FindByIdAsync(id);
                if (rol == null) return NotFound();

                rol.Name = model.Nombre;
                rol.Descripcion = model.Descripcion;

                var result = await _roleManager.UpdateAsync(rol);

                if (result.Succeeded)
                    return RedirectToAction(nameof(Index));

                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }

        // GET: Roles/Delete/5
        [AllowAnonymous]
        
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();

            var rol = await _roleManager.FindByIdAsync(id);
            if (rol == null) return NotFound();

            var model = new RolViewModel
            {
                RolId = rol.Id,
                Nombre = rol.Name!,
                Descripcion = rol.Descripcion
            };

            return View(model);
        }

        // POST: Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var rol = await _roleManager.FindByIdAsync(id);
            if (rol != null)
            {
                var result = await _roleManager.DeleteAsync(rol);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError("", error.Description);

                    return View(new RolViewModel { RolId = rol.Id, Nombre = rol.Name!, Descripcion = rol.Descripcion });
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
