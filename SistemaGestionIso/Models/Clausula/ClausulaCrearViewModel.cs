namespace SistemaGestionIso.Models.Clausulas
{
    public class ClausulaCrearViewModel
    {
        public int Id { get; set; }
        public required string Codigo { get; set; }
        public required string Descripcion { get; set; }
        public int NormaIsoId { get; set; }
        public string NombreNormaIso { get; set; } 
        public int Orden { get; set; }
    }
}
