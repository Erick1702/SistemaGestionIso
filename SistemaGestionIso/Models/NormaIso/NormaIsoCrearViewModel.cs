using System.ComponentModel.DataAnnotations;

namespace SistemaGestionIso.Models.NormaIso
{
    public class NormaIsoCrearViewModel
    {
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public required string Nombre { get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public required string Version { get; set; }
        public string Descripcion { get; set; }
    }
}
