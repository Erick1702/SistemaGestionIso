namespace SistemaGestionIso.Entidades
{
    public class Cumplimiento
    {
        public int Id { get; set; }
        public int RequisitoId { get; set; }
        public Requisito? Requisito { get; set; }
        public EstadoCumplimiento Estado { get; set; }
        public DateTime Fecha { get; set; }
        public string Observaciones { get; set; } = "";

        //Agregar usuario resposable
        public List<Evidencia> Evidencias { get; set; }
    }
}
