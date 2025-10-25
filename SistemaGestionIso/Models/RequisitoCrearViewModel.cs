using System.ComponentModel.DataAnnotations;

namespace SistemaGestionIso.Models
{
    public class RequisitoCrearViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "La cláusula es obligatoria")]
        [Display(Name = "Cláusula")]
        public int ClausulaId { get; set; }

        public string? CodigoClausula { get; set; }
        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(550, ErrorMessage = "La descripción no puede exceder {1} caracteres")]
        [Display(Name = "Descripción del Requisito")]
        public string? Descripcion { get; set; }


        public string? DescripcionClausula { get; set; }

        public string? NombreNormaIso { get; set; }

        public int? NormaIsoId { get; set; }
    }
}
