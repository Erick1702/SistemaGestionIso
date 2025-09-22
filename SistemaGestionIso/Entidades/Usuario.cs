using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SistemaGestionIso.Entidades
{
    public class Usuario:IdentityUser
    {
        [Required]
        [StringLength(150)]
        public string PrimerNombre { get; set; }
        
        [StringLength(150)]
        public string? SegundoNombre { get; set; } = "";
        [Required]
        [StringLength(150)]
        public string PrimerApellido { get; set; }
        [Required]
        [StringLength(150)]
        public string SegundoApellido { get; set; }
        public string NombreCompleto => $"{PrimerNombre} {SegundoNombre} {PrimerApellido} {SegundoApellido}";

        public string? Celular { get; set; }= "";

        public List<Cumplimiento> Cumplimientos { get; set; }
        public List<Evidencia> Evidencias { get; set; }
        public List<NoConformidad> NoConformidades { get; set; }
        public List<AccionCorrectiva> AccionCorrectivas { get; set; }
        public List<Auditoria> Auditorias { get; set; }



    }
}
