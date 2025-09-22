namespace SistemaGestionIso.Entidades
{
    public class Requisito
    {
        public int Id { get; set; }
        public int ClausulaId { get; set; }
        public Clausula Clausula { get; set; }
        public string Descripcion { get; set; }= "";
        public List<Cumplimiento> Cumplimientos { get; set; }
        public List<Hallazgo> Hallazgos { get; set; }
    }
}
