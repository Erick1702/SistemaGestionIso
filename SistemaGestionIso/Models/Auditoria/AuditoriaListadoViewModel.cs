namespace SistemaGestionIso.Models.Auditoria
{
    public class AuditoriaListadoViewModel
    {
        public IEnumerable<AuditoriaViewModel> Auditorias { get; set; } = new List<AuditoriaViewModel>();
        public string? Mensaje { get; set; }

        // Estadísticas
        public int TotalAuditorias => Auditorias.Count();
        public int AuditoriasProgramadas => Auditorias.Count(a => a.EstadoAuditoria == "Programada");
        public int AuditoriasEnProgreso => Auditorias.Count(a => a.EstadoAuditoria == "En Progreso");
        public int AuditoriasFinalizadas => Auditorias.Count(a => a.EstadoAuditoria == "Finalizada");
        public int TotalHallazgos => Auditorias.Sum(a => a.CantidadHallazgos);
    }
}
