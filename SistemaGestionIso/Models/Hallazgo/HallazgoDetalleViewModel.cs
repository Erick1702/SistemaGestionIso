namespace SistemaGestionIso.Models.Hallazgo
{
    public class HallazgoDetalleViewModel
    {
        public int Id { get; set; }
        public int AuditoriaId { get; set; }
        public string AreaAuditada { get; set; } = "";
        public string TipoAuditoria { get; set; } = "";
        public DateTime FechaInicioAuditoria { get; set; }
        public DateTime FechaFinAuditoria { get; set; }

        public int RequisitoId { get; set; }
        public string DescripcionRequisito { get; set; } = "";
        public string CodigoClausula { get; set; } = "";
        public string DescripcionClausula { get; set; } = "";
        public string NombreNormaIso { get; set; } = "";

        public string Tipo { get; set; } = "";
        public string TipoColor { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public string? EvidenciaPath { get; set; }
        public string? NombreArchivoEvidencia { get; set; }

        // Auditor
        public string? NombreAuditor { get; set; }
        public string? EmailAuditor { get; set; }
    }
}
