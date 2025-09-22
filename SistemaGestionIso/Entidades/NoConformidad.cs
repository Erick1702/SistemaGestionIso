using System.ComponentModel.DataAnnotations;

namespace SistemaGestionIso.Entidades
{
    public class NoConformidad
    {
        public int Id { get; set; }
        [StringLength(250)]
        [Required]
        public string Descripcion { get; set; }
        [StringLength(250)]
        [Required]
        public string Origen { get; set; } // Auditoria, Inspeccion, Queja, Otro
        [StringLength(50)]
        [Required]
        public string Severidad { get; set; } // Baja, Media, Alta
        public DateTime FechaDeteccion { get; set; }
        public DateTime? FechaCierre { get; set; }
        public string? UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        public List<AccionCorrectiva> AccionCorrectivas { get; set; }
        

       
    }
}
