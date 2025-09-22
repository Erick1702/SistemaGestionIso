using System.ComponentModel.DataAnnotations;

namespace SistemaGestionIso.Entidades
{
    public class Auditoria
    {
        public int Id { get; set; }
        public DateTime FechaIni { get; set; }
        public DateTime FechaFin { get; set; }
        [StringLength(250)]
        public string AreaAuditada { get; set; }
        [StringLength(250)]
        public string TipoAuditoria { get; set; } // Interna, Externa, de Proveedores
        public string? UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
        //Poner auditor responsable de la auditoria ¿Es igual que usuario?

        public List<Hallazgo> Hallazgos { get; set; }
    }
}
