using SistemaGestionIso.Entidades;

namespace SistemaGestionIso.Models.Cumplimiento
{
    public class CumplimientoListadoViewModel
    {
        public IEnumerable<CumplimientoViewModel> Cumplimientos { get; set; } = new List<CumplimientoViewModel>();
        public string? Mensaje { get; set; }

        // Estadísticas
        public int TotalCumple => Cumplimientos.Count(c => c.Estado == EstadoCumplimiento.Cumple);
        public int TotalParcial => Cumplimientos.Count(c => c.Estado == EstadoCumplimiento.Parcial);
        public int TotalNoCumple => Cumplimientos.Count(c => c.Estado == EstadoCumplimiento.NoCumple);
        public double PorcentajeCumplimiento => Cumplimientos.Any()
            ? Math.Round((TotalCumple * 100.0) / Cumplimientos.Count(), 1)
            : 0;
    }
}
