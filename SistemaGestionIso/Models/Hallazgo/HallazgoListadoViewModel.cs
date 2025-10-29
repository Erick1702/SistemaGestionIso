namespace SistemaGestionIso.Models.Hallazgo
{
    public class HallazgoListadoViewModel
    {
        public IEnumerable<HallazgoViewModel> Hallazgos { get; set; } = new List<HallazgoViewModel>();
        public string? Mensaje { get; set; }

        // Info de la auditoría (cuando se lista por auditoría)
        public int? AuditoriaId { get; set; }
        public string? AreaAuditada { get; set; }
        public string? TipoAuditoria { get; set; }

        // Estadísticas
        public int TotalConformidades => Hallazgos.Count(h => h.Tipo == "Conformidad");
        public int TotalNoConformidades => Hallazgos.Count(h => h.Tipo == "No Conformidad");
        public int TotalObservaciones => Hallazgos.Count(h => h.Tipo == "Observación");
    }
}
