namespace SistemaGestionIso.Models.NormaIso
{
    public class NormaIsoDetalleViewModel
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }
        public required string Version { get; set; }
        public string Descripcion { get; set; }
    }
}
