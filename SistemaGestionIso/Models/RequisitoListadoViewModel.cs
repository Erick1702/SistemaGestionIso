namespace SistemaGestionIso.Models
{
    public class RequisitoListadoViewModel
    {
        public IEnumerable<RequisitoViewModel> Requisitos { get; set; } = [];
        public string? Mensaje { get; set; }
    }
}
