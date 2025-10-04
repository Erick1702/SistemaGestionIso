using Microsoft.AspNetCore.Identity;
using SistemaGestionIso.Entidades;
using System.Security.Claims;

namespace SistemaGestionIso.Servicios
{
    public interface IServicioUsuarios
    {
        string? ObtenerUsuarioId();
    }

    public class ServicioUsuarios : IServicioUsuarios
    {

        private readonly UserManager<Usuario> userManager;
        private readonly HttpContext httpContext;

        public ServicioUsuarios(IHttpContextAccessor httpContextAccessor, UserManager<Usuario> userManager)
        {

            this.userManager = userManager;
            httpContext = httpContextAccessor.HttpContext;
        }

        public string? ObtenerUsuarioId()
        {
            var idClaim = httpContext.User.Claims
                .Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault();

            if (idClaim is null)
            {
                return null;
            }
            return idClaim.Value;
        }
    }
}
