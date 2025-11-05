namespace SistemaGestionIso.Models.NormaIso
{
    public class NormaIsoListadoViewModel
    {
        public IEnumerable<NormaIsoViewModel> NormasIso { get; set; } = [];
        public string Mensaje { get; set; }
    }
}
