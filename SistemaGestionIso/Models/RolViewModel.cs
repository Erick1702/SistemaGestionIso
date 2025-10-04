using System.ComponentModel.DataAnnotations;

namespace SistemaGestionIso.Models
{
    public class RolViewModel
    {
        public string? RolId { get; set; }   // Necesario para Edit y Delete

        [Required(ErrorMessage = "El nombre del rol es obligatorio")]
        [StringLength(100)]
        [Display(Name = "Nombre del Rol")]
        public string Nombre { get; set; } = string.Empty;

        //[StringLength(250)]
        //[Display(Name = "Descripción")]
        //public string? Descripcion { get; set; }
    }
}
