using SistemaGestionIso.Entidades;

namespace SistemaGestionIso.Models.Cumplimiento
{
    public class CumplimientoDetalleViewModel
    {
        public int Id { get; set; }
        public int RequisitoId { get; set; }
        public string? DescripcionRequisito { get; set; }
        public string? CodigoClausula { get; set; }
        public string? DescripcionClausula { get; set; }
        public string? NombreNormaIso { get; set; }
        public EstadoCumplimiento Estado { get; set; }
        public string EstadoTexto { get; set; }
        public string EstadoColor { get; set; }
        public DateTime Fecha { get; set; }
        public string Observaciones { get; set; } = "";
        public int CantidadEvidencias { get; set; }

        // Para mostrar las evidencias
        public List<EvidenciaSimpleViewModel> Evidencias { get; set; } = new();
    }
}
