namespace SistemaGestionIso.Models
{
    public class UsuarioRolesUsuarioViewModel
    {
        public string UsuarioId { get; set; }
        public string Email { get; set; }
        public IEnumerable<UsuarioRolViewModel> Roles { get; set; } = [];
    }
}
