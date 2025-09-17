namespace SistemaGestionIso.Entidades
{
    public class DocumentoVersion
    {
        public int Id { get; set; }
        public int DocumentoSGIId { get; set; }
        public DocumentoSGI DocumentoSGI { get; set; }
        public int Version { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string Autor { get; set; }
        public string RutaArchivo { get; set; }
    }
}
