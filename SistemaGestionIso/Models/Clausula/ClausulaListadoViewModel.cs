namespace SistemaGestionIso.Models.Clausula
{
    public class ClausulaListadoViewModel
    {
        public IEnumerable<ClausulaViewModel> Clausulas { get; set; } = [];
        public string Mensaje { get; set; }
    }
}
