namespace SistemaGestionIso.Models.NormasIso
{
    public class NormaIsoListadoViewModel
    {
        public IEnumerable<NormaIsoViewModel> NormasIso { get; set; } = [];
        public string Mensaje { get; set; }
    }
}
