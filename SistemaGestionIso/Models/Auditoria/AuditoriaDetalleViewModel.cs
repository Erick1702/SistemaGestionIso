namespace SistemaGestionIso.Models.Auditoria
{
    public class AuditoriaDetalleViewModel
    {
        public int Id { get; set; }
        public DateTime FechaIni { get; set; }
        public DateTime FechaFin { get; set; }
        public string AreaAuditada { get; set; } = "";
        public string TipoAuditoria { get; set; } = "";

        // Auditor
        public string? UsuarioId { get; set; }
        public string? NombreAuditor { get; set; }
        public string? EmailAuditor { get; set; }

        // Estado
        public string EstadoAuditoria { get; set; } = "";
        public string ColorEstado { get; set; } = "";
        public int DuracionDias { get; set; }

        // Hallazgos
        public List<HallazgoSimpleViewModel> Hallazgos { get; set; } = new();

        // Estadísticas
        public int CantidadHallazgos { get; set; }
        public int CantidadConformidades { get; set; }
        public int CantidadNoConformidades { get; set; }
        public int CantidadObservaciones { get; set; }
    }
}
