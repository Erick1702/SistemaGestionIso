namespace SistemaGestionIso.Models.Auditoria
{
    public class HallazgoSimpleViewModel
    {
        public int Id { get; set; }
        public string Tipo { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public string? DescripcionRequisito { get; set; }
        public string? CodigoClausula { get; set; }
        public bool TieneEvidencia { get; set; }
    }
}
