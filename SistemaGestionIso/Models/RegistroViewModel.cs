using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SistemaGestionIso.Models.Requisitos
{
    public class RegistroViewModel
    {
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [DisplayName("Primer Nombre")]
        [StringLength(maximumLength: 150, MinimumLength = 1, ErrorMessage = "La longuitud del {0}  campo entre {2} y {1}")]
        public string PrimerNombre { get; set; }
        
        //[DisplayName("Segundo Nombre")]
        //[StringLength(maximumLength: 150, MinimumLength = 0, ErrorMessage = "La longuitud del {0}  campo entre {2} y {1}")]
        public string SegundoNombre { get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [DisplayName("Apellido Paterno")]
        [StringLength(maximumLength: 150, MinimumLength = 1, ErrorMessage = "La longuitud del {0}  campo entre {2} y {1}")]
        public string PrimerApellido { get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [DisplayName("Apellido Materno")]
        [StringLength(maximumLength: 150, MinimumLength = 1, ErrorMessage = "La longuitud del {0}  campo entre {2} y {1}")]
        public string SegundoApellido { get; set; }
        public string Celular { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [EmailAddress(ErrorMessage = "El campo {0} debe ser un correo electrónico válido.")]
        [DisplayName("Correo Electrónico")]
        public string Email { get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [DataType(DataType.Password)]
        [DisplayName("Contraseña")]
        public string Password { get; set; }

    }
}
