using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaGestionIso.Entidades;
using SistemaGestionIso.Models;
using SistemaGestionIso.Servicios;

namespace SistemaGestionIso.Controllers
{
    public class UsuariosController: Controller
    {
        private readonly UserManager<Usuario> userManager;
        private readonly SignInManager<Usuario> signInManager;
        private readonly ApplicationDbContext context;
        private readonly IConfiguration config;

        public UsuariosController(UserManager<Usuario> userManager,
            SignInManager<Usuario> signInManager, ApplicationDbContext context, IConfiguration config)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.context = context;
            this.config = config;
        }

        [AllowAnonymous]
        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Registro(RegistroViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }
            var usuario = new Usuario
            {
                PrimerNombre = modelo.PrimerNombre,
                SegundoNombre = modelo.SegundoNombre,
                PrimerApellido = modelo.PrimerApellido,
                SegundoApellido = modelo.SegundoApellido,
                UserName = modelo.Email,
                Email = modelo.Email,
                Celular = modelo.Celular
            };
            var resultado = await userManager.CreateAsync(usuario, password:modelo.Password);
            if (resultado.Succeeded)
            {
                await signInManager.SignInAsync(usuario, isPersistent: true);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(modelo);
            }
        }


        [AllowAnonymous]
        public IActionResult Login( string? mensaje = null)
        {
            if (mensaje is not null)
            {
                ViewData["Mensaje"] = mensaje;
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }
            var resultado = await signInManager.PasswordSignInAsync(modelo.Email, modelo.Password, modelo.Recuerdame, 
                 lockoutOnFailure: false);
            if (resultado.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Correo Electronico o Contraseña incorrecto. Intente nuevamente.");
                return View(modelo);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        //[Authorize(Roles = Constantes.RolAdministrador)]
        public async Task<IActionResult> Listado(string? mensaje = null)
        {
            var usuarios = await context.Users.Select(x => new UsuarioViewModel
            {
                Id = x.Id,
                Email = x.Email!, //Significa que no es null
                PrimerNombre = x.PrimerNombre + " " + (string.IsNullOrEmpty(x.SegundoNombre) ? "" : x.SegundoNombre + " ") + x.PrimerApellido + " " + x.SegundoApellido
            }).ToListAsync();

            var modelo = new UsuarioListadoViewModel();

            modelo.Usuarios = usuarios;
            modelo.Mensaje = mensaje;
            return View(modelo);
        }

        [HttpGet]
        //[Authorize(Roles = Constantes.RolAdministrador)]
        public async Task<IActionResult> RolesUsuario(string usuarioId)
        {

            var usuario = await userManager.FindByIdAsync(usuarioId); // Buscar por Id

            if (usuario is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            //Roles del usuario
            var rolesQueElUsuarioTiene = await userManager.GetRolesAsync(usuario);
            //Roles existentes
            var rolesExistentes = await context.Roles.ToListAsync();

            var rolesDelUsuario = rolesExistentes.Select(x => new UsuarioRolViewModel
            {
                Nombre = x.Name!,
                TieneRol = rolesQueElUsuarioTiene.Contains(x.Name!)
            });

            var modelo = new UsuarioRolesUsuarioViewModel
            {
                UsuarioId = usuario.Id,
                Email = usuario.Email!,
                Roles = rolesDelUsuario.OrderBy(x => x.Nombre)
            };

            return View(modelo);

        }

        [HttpPost]
        //[Authorize(Roles = Constantes.RolAdministrador)]
        public async Task<IActionResult> EditarRoles(EditarRolesViewModel modelo)
        {
            var usuario = await userManager.FindByIdAsync(modelo.UsuarioId);

            if (usuario is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await context.UserRoles.Where(x => x.UserId == usuario.Id).ExecuteDeleteAsync();

            await userManager.AddToRolesAsync(usuario, modelo.RolesSeleccionados);

            var mensaje = $"Los roles del {usuario.Email} han sido actualizados";
        
            return RedirectToAction("Listado", new { mensaje });
        }

    }
}
