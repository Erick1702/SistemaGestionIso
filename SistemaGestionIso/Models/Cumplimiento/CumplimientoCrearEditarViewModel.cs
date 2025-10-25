using SistemaGestionIso.Entidades;
using System.ComponentModel.DataAnnotations;

namespace SistemaGestionIso.Models.Cumplimiento
{
    public class CumplimientoCrearEditarViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El requisito es obligatorio")]
        [Display(Name = "Requisito")]
        public int RequisitoId { get; set; }

        public string? DescripcionRequisito { get; set; }

        public string? CodigoClausula { get; set; }

        public string? NombreNormaIso { get; set; }

        public int? ClausulaId { get; set; }

        public int? NormaIsoId { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio")]
        [Display(Name = "Estado de Cumplimiento")]
        public EstadoCumplimiento Estado { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria")]
        [Display(Name = "Fecha de Evaluación")]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Las observaciones son obligatorias")]
        [StringLength(1000, ErrorMessage = "Las observaciones no pueden exceder {1} caracteres")]
        [Display(Name = "Observaciones")]
        public string Observaciones { get; set; } = "";

        // Usuario Responsable
        [Display(Name = "Usuario Responsable")]
        public string? UsuarioResponsableId { get; set; }

        public string? NombreUsuarioResponsable { get; set; }
    }
}
