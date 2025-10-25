namespace SistemaGestionIso.Models.Evidencia
{
    public class EvidenciaDetalleViewModel
    {
        public int Id { get; set; }
        public int CumplimientoId { get; set; }
        public string NombreArchivo { get; set; } = "";
        public string RutaArchivo { get; set; } = "";
        public DateTime FechaSubida { get; set; }

        // Usuario
        public string? UsuarioId { get; set; }
        public string? NombreUsuario { get; set; }
        public string? EmailUsuario { get; set; }

        // Cumplimiento
        public string EstadoCumplimiento { get; set; } = "";
        public DateTime FechaCumplimiento { get; set; }
        public string ObservacionesCumplimiento { get; set; } = "";

        // Requisito
        public int RequisitoId { get; set; }
        public string? DescripcionRequisito { get; set; }

        // Cláusula
        public string? CodigoClausula { get; set; }
        public string? DescripcionClausula { get; set; }

        // Norma ISO
        public string? NombreNormaIso { get; set; }

        // Archivo
        public long TamañoArchivo { get; set; }
        public string TamañoArchivoFormateado { get; set; } = "";
        public string ExtensionArchivo { get; set; } = "";
        public string TipoMime { get; set; } = "";
    }
}
