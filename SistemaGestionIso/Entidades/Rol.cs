using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SistemaGestionIso.Entidades
{
    public class Rol: IdentityRole
    {
        
        [StringLength(150)]
        public string? Descripcion { get; set; }
    }
}
