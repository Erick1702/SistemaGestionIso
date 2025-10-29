using System.ComponentModel.DataAnnotations;

namespace SistemaGestionIso.Models.Hallazgo
{
    public class HallazgoViewModel
    {
        public int Id { get; set; }

        public int AuditoriaId { get; set; }

        [Display(Name = "Área Auditada")]
        public string? AreaAuditada { get; set; }

        [Display(Name = "Tipo de Auditoría")]
        public string? TipoAuditoria { get; set; }

        public DateTime? FechaAuditoria { get; set; }

        public int RequisitoId { get; set; }

        [Display(Name = "Requisito")]
        public string? DescripcionRequisito { get; set; }

        [Display(Name = "Cláusula")]
        public string? CodigoClausula { get; set; }

        [Display(Name = "Norma ISO")]
        public string? NombreNormaIso { get; set; }

        [Display(Name = "Tipo de Hallazgo")]
        public string Tipo { get; set; } = "";

        public string TipoColor => Tipo switch
        {
            "Conformidad" => "success",
            "No Conformidad" => "danger",
            "Observación" => "warning",
            _ => "secondary"
        };

        [Display(Name = "Descripción")]
        public string Descripcion { get; set; } = "";

        [Display(Name = "Evidencia")]
        public string? EvidenciaPath { get; set; }

        public bool TieneEvidencia => !string.IsNullOrEmpty(EvidenciaPath);
    }
}
