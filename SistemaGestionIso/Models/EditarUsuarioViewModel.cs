using System.ComponentModel.DataAnnotations;

namespace SistemaGestionIso.Models
{
    public class EditarUsuarioViewModel
    {
        public string Id { get; set; } = null!;

        [Required(ErrorMessage = "El primer nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El primer nombre no puede tener más de 100 caracteres")]
        [Display(Name = "Primer Nombre")]
        public string PrimerNombre { get; set; } = null!;

        [StringLength(100, ErrorMessage = "El segundo nombre no puede tener más de 100 caracteres")]
        [Display(Name = "Segundo Nombre")]
        public string? SegundoNombre { get; set; }

        [Required(ErrorMessage = "El primer apellido es obligatorio")]
        [StringLength(100, ErrorMessage = "El primer apellido no puede tener más de 100 caracteres")]
        [Display(Name = "Primer Apellido")]
        public string PrimerApellido { get; set; } = null!;

        [StringLength(100, ErrorMessage = "El segundo apellido no puede tener más de 100 caracteres")]
        [Display(Name = "Segundo Apellido")]
        public string? SegundoApellido { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido")]
        [Display(Name = "Correo Electrónico")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "El celular es obligatorio")]
        [Phone(ErrorMessage = "El formato del celular no es válido")]
        [StringLength(20, ErrorMessage = "El celular no puede tener más de 20 caracteres")]
        [Display(Name = "Celular")]
        public string Celular { get; set; } = null!;
    }
}
