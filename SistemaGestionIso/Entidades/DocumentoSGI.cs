using System.ComponentModel.DataAnnotations;

namespace SistemaGestionIso.Entidades
{
    public class DocumentoSGI
    {
        public int Id { get; set; }
        [StringLength(250)]
        [Required]
        public string Codigo { get; set; }
        [StringLength(250)]
        [Required]
        public string Titulo { get; set; }
        [StringLength(250)]
        [Required]
        public string Tipo { get; set; } // Manual, Procedimiento, Instructivo, Formato
        [Required]
        public bool Estado { get; set; } // Vigente, Obsoleto

        public List<DocumentoVersion> DocumentoVersiones { get; set; }

    }
}
