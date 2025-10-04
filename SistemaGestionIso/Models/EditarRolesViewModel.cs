namespace SistemaGestionIso.Models
{
    public class EditarRolesViewModel
    {
        public required string UsuarioId { get; set; }
        public List<string> RolesSeleccionados { get; set; } = [];
    }
}
