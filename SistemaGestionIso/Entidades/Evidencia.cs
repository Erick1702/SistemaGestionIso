using System.Globalization;

namespace SistemaGestionIso.Entidades
{
    public class Evidencia
    {
        public int Id { get; set; }
        public int CumplimientoId { get; set; }
        public Cumplimiento Cumplimiento { get; set; }
        public string RutaArchivo { get; set; } = "";
        public string NombreArchivo { get; set; } = "";
        public DateTime FechaSubida { get; set; }
        public String? UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        //Agregar usuario que sube la evidencia
    }
}
