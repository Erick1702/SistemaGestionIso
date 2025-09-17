using System.ComponentModel.DataAnnotations;

namespace SistemaGestionIso.Entidades
{
    public class Clausula
    {
        public int Id { get; set; }
        public int NormaIsoId { get; set; }
        public NormaIso NormaIso { get; set; }
        [StringLength(50)]
        [Required]
        public string Codigo { get; set; }
        [StringLength(50)]
        [Required]
        public string Descripcion { get; set; }
        public int Orden { get; set; }
        public List<Requisito> Requisitos { get; set; }


    }
}
