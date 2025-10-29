namespace SistemaGestionIso.Models.Hallazgo
{
    public class RequisitoDropdownViewModel
    {
        public int Id { get; set; }
        public string Descripcion { get; set; } = "";
        public string CodigoClausula { get; set; } = "";
        public string NombreNormaIso { get; set; } = "";
        public string TextoCompleto => $"{CodigoClausula} - {Descripcion} ({NombreNormaIso})";
    }
}
