namespace SistemaGestionIso.Models.Clausulas
{
    public class ClausulaListadoViewModel
    {
        public IEnumerable<ClausulaViewModel> Clausulas { get; set; } = [];
        public string Mensaje { get; set; }
    }
}
