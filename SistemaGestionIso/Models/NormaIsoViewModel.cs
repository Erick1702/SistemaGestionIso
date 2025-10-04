using System.ComponentModel.DataAnnotations;

namespace SistemaGestionIso.Models
{
    public class NormaIsoViewModel
    {

        public int NormaISOId { get; set; }

        [Required(ErrorMessage = "El nombre de la norma es obligatorio")]
        [StringLength(200)]
        [Display(Name = "Nombre de la Norma ISO")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La versión es obligatoria")]
        [StringLength(50)]
        [Display(Name = "Versión")]
        public string Version { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }
    }
}
