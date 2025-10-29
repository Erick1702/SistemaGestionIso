using System.ComponentModel.DataAnnotations;

namespace SistemaGestionIso.Models.Auditoria
{
    public class AuditoriaViewModel
    {

        public int Id { get; set; }

        [Display(Name = "Fecha de Inicio")]
        public DateTime FechaIni { get; set; }

        [Display(Name = "Fecha de Fin")]
        public DateTime FechaFin { get; set; }

        [Display(Name = "Área Auditada")]
        public string AreaAuditada { get; set; } = "";

        [Display(Name = "Tipo de Auditoría")]
        public string TipoAuditoria { get; set; } = "";

        // Auditor
        public string? UsuarioId { get; set; }
        public string? NombreAuditor { get; set; }
        public string? EmailAuditor { get; set; }

        // Estadísticas
        public int CantidadHallazgos { get; set; }
        public int CantidadConformidades { get; set; }
        public int CantidadNoConformidades { get; set; }
        public int CantidadObservaciones { get; set; }

        // Propiedades calculadas
        public int DuracionDias => (FechaFin - FechaIni).Days + 1;
        public string EstadoAuditoria => DateTime.Now < FechaIni ? "Programada" :
                                         DateTime.Now > FechaFin ? "Finalizada" : "En Progreso";
        public string ColorEstado => EstadoAuditoria switch
        {
            "Programada" => "info",
            "En Progreso" => "warning",
            "Finalizada" => "secondary",
            _ => "secondary"
        };
    }
}
