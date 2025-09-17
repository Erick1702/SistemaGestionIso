namespace SistemaGestionIso.Entidades
{
    public class Auditoria
    {
        public int Id { get; set; }
        public DateTime FechaIni { get; set; }
        public DateTime FechaFin { get; set; }
        public string AreaAuditada { get; set; }
        //Poner auditor responsable de la auditoria ¿Es igual que usuario?
    }
}
