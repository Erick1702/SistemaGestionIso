namespace SistemaGestionIso.Models.Evidencia
{
    public class EvidenciaListadoViewModel
    {
        public IEnumerable<EvidenciaViewModel> Evidencias { get; set; } = new List<EvidenciaViewModel>();
        public string? Mensaje { get; set; }

        // Info del cumplimiento (cuando se lista por cumplimiento)
        public int? CumplimientoId { get; set; }
        public string? EstadoCumplimiento { get; set; }
        public DateTime? FechaCumplimiento { get; set; }
    }
}
