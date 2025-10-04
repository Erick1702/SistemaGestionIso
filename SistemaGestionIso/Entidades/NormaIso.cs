using System.ComponentModel.DataAnnotations;

namespace SistemaGestionIso.Entidades
{
    public class NormaIso
    {
        public int Id { get; set; }
        [StringLength(250)]
        [Required]
        public string Nombre { get; set; }
        [StringLength(50)]
        [Required]
        public string Version { get; set; }
        public string? Descripcion { get; set; }
        public List<Clausula> Clausulas { get; set; }
    }
}
