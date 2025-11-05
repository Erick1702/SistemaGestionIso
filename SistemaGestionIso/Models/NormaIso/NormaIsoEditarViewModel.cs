using System.ComponentModel.DataAnnotations;

namespace SistemaGestionIso.Models.NormasIso
{
    public class NormaIsoEditarViewModel
    {
        public int Id { get; set; }
        [StringLength(250)]
        [Required]
        public string Nombre { get; set; }
        [StringLength(50)]
        [Required]
        public string Version { get; set; }
        public string Descripcion { get; set; }
    }
}
