using System.ComponentModel.DataAnnotations;

namespace SistemaGestionIso.Models.Evidencia
{
    public class EvidenciaSubirViewModel
    {
        [Required(ErrorMessage = "El cumplimiento es obligatorio")]
        public int CumplimientoId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un archivo")]
        [Display(Name = "Archivo")]
        public IFormFile Archivo { get; set; }

        [StringLength(500, ErrorMessage = "La descripción no puede exceder {1} caracteres")]
        [Display(Name = "Descripción (opcional)")]
        public string? Descripcion { get; set; }

        // Datos de contexto (para mostrar en la vista)
        public string? DescripcionRequisito { get; set; }
        public string? CodigoClausula { get; set; }
        public string? NombreNormaIso { get; set; }
        public string? EstadoCumplimiento { get; set; }
    }
}
