namespace SistemaGestionIso.Entidades
{
    public class Hallazgo
    {
        public int Id { get; set; }
        public int AuditoriaId { get; set; }
        public Auditoria Auditoria { get; set; }
        public int RequisitoId { get; set; }
        public Requisito Requisito { get; set; }
        public string Tipo { get; set; } = ""; // Conformidad, No Conformidad, Observacion
        public string Descripcion { get; set; } = "";
        public string? EvidenciaPath { get; set; } = "";
    }
}
