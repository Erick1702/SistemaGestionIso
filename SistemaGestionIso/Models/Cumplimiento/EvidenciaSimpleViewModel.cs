namespace SistemaGestionIso.Models.Cumplimiento
{
    public class EvidenciaSimpleViewModel
    {
        public int Id { get; set; }
        public string NombreArchivo { get; set; } = "";
        public string RutaArchivo { get; set; } = "";
        public DateTime FechaSubida { get; set; }
        public string? NombreUsuario { get; set; }
    }
}
