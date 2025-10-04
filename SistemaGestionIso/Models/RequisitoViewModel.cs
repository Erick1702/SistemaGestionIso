using SistemaGestionIso.Entidades;
using System.ComponentModel.DataAnnotations;

namespace SistemaGestionIso.Models
{
    public class RequisitoViewModel
    {
        public int Id { get; set; }

        public int ClausulaId { get; set; }
        public Clausula? Clausula { get; set; }
        [StringLength(50)]
        public string CodigoClausula { get; set; }
        [StringLength(550)]
        public string? Descripcion { get; set; }
    }
}
