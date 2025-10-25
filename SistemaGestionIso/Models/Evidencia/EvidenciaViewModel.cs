using System.ComponentModel.DataAnnotations;

namespace SistemaGestionIso.Models.Evidencia
{
    public class EvidenciaViewModel
    {
        public int Id { get; set; }

        public int CumplimientoId { get; set; }

        [Display(Name = "Nombre del Archivo")]
        public string NombreArchivo { get; set; } = "";

        [Display(Name = "Ruta del Archivo")]
        public string RutaArchivo { get; set; } = "";

        [Display(Name = "Fecha de Subida")]
        public DateTime FechaSubida { get; set; }

        // Usuario
        public string? UsuarioId { get; set; }
        public string? NombreUsuario { get; set; }
        public string? EmailUsuario { get; set; }

        // Información del Cumplimiento
        public string? EstadoCumplimiento { get; set; }
        public DateTime? FechaCumplimiento { get; set; }

        // Información del Requisito
        public string? DescripcionRequisito { get; set; }
        public string? CodigoClausula { get; set; }
        public string? NombreNormaIso { get; set; }

        // Propiedades calculadas
        public string TamañoArchivoFormateado { get; set; } = "";
        public string ExtensionArchivo => Path.GetExtension(NombreArchivo).ToLower();
        public string IconoArchivo => ExtensionArchivo switch
        {
            ".pdf" => "bi-file-earmark-pdf",
            ".doc" or ".docx" => "bi-file-earmark-word",
            ".xls" or ".xlsx" => "bi-file-earmark-excel",
            ".jpg" or ".jpeg" or ".png" or ".gif" => "bi-file-earmark-image",
            ".zip" or ".rar" => "bi-file-earmark-zip",
            _ => "bi-file-earmark"
        };
    }
}
