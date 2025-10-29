using System.ComponentModel.DataAnnotations;

namespace SistemaGestionIso.Models.Hallazgo
{
    public class HallazgoCrearEditarViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La auditoría es obligatoria")]
        public int AuditoriaId { get; set; }

        public string? AreaAuditada { get; set; }
        public string? TipoAuditoria { get; set; }

        [Required(ErrorMessage = "El requisito es obligatorio")]
        [Display(Name = "Requisito")]
        public int RequisitoId { get; set; }

        public string? DescripcionRequisito { get; set; }
        public string? CodigoClausula { get; set; }
        public string? NombreNormaIso { get; set; }

        [Required(ErrorMessage = "El tipo de hallazgo es obligatorio")]
        [Display(Name = "Tipo de Hallazgo")]
        public string Tipo { get; set; } = "";

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(1000, ErrorMessage = "La descripción no puede exceder {1} caracteres")]
        [Display(Name = "Descripción del Hallazgo")]
        public string Descripcion { get; set; } = "";

        [Display(Name = "Evidencia (Archivo)")]
        public IFormFile? EvidenciaArchivo { get; set; }

        public string? EvidenciaPath { get; set; }

        // Lista de requisitos para el dropdown
        public List<RequisitoDropdownViewModel>? Requisitos { get; set; }
    }
}
