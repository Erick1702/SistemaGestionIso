using System.ComponentModel.DataAnnotations;

namespace SistemaGestionIso.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [EmailAddress(ErrorMessage = "El campo {0} debe ser un correo electrónico válido")]
        [Display(Name = "Correo Electrónico")]
        public string Email { get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } 
        public bool Recuerdame { get; set; }
    }
}
