using System.ComponentModel.DataAnnotations;

namespace SistemaGestionIso.Models.Clausula
{
    public class ClausulaViewModel
    {
        public int Id { get; set; }
        
        [StringLength(50)]
        [Required(ErrorMessage = "El código es obligatorio")]
        public string Codigo { get; set; }
        [StringLength(250)]
        [Required(ErrorMessage = "La descripción es obligatoria")]
        public string Descripcion { get; set; }
        public int NormaIsoId { get; set; }
        public string NombreNormaIso { get; set; }
        public int Orden { get; set; }
    }
}
