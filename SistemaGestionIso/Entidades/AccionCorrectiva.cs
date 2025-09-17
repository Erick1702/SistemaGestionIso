namespace SistemaGestionIso.Entidades
{
    public class AccionCorrectiva
    {
        public int Id { get; set; }
        public int NoConformidadId { get; set; }
        public NoConformidad NoConformidad { get; set; }
        public string Accion { get; set; }
        public DateTime FechaCorrectiva { get; set; }
        public DateTime FechaCierre { get; set; }
        public string? Evidencia { get; set; }

        //Falta usuario que realiza la accion correctiva
    }
}
