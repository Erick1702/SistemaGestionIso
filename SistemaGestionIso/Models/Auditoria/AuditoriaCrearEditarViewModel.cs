using System.ComponentModel.DataAnnotations;

namespace SistemaGestionIso.Models.Auditoria
{
    public class AuditoriaCrearEditarViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        [Display(Name = "Fecha de Inicio")]
        [DataType(DataType.Date)]
        public DateTime FechaIni { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "La fecha de fin es obligatoria")]
        [Display(Name = "Fecha de Fin")]
        [DataType(DataType.Date)]
        public DateTime FechaFin { get; set; } = DateTime.Now.AddDays(7);

        [Required(ErrorMessage = "El área auditada es obligatoria")]
        [StringLength(250, ErrorMessage = "El área no puede exceder {1} caracteres")]
        [Display(Name = "Área Auditada")]
        public string AreaAuditada { get; set; } = "";

        [Required(ErrorMessage = "El tipo de auditoría es obligatorio")]
        [Display(Name = "Tipo de Auditoría")]
        public string TipoAuditoria { get; set; } = "";

        [Display(Name = "Auditor Responsable")]
        public string? UsuarioId { get; set; }

        public string? NombreAuditor { get; set; }
    }
}
