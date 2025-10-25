using SistemaGestionIso.Entidades;
using System.ComponentModel.DataAnnotations;

namespace SistemaGestionIso.Models.Cumplimiento
{
    public class CumplimientoViewModel
    {
        public int Id { get; set; }

        public int RequisitoId { get; set; }

        public string? DescripcionRequisito { get; set; }

        public string? CodigoClausula { get; set; }

        public string? NombreNormaIso { get; set; }

        public int? ClausulaId { get; set; }

        public int? NormaIsoId { get; set; }

        [Display(Name = "Estado")]
        public EstadoCumplimiento Estado { get; set; }

        public string EstadoTexto => Estado switch
        {
            EstadoCumplimiento.Cumple => "Cumple",
            EstadoCumplimiento.Parcial => "Cumple Parcialmente",
            EstadoCumplimiento.NoCumple => "No Cumple",
            _ => "Desconocido"
        };

        public string EstadoColor => Estado switch
        {
            EstadoCumplimiento.Cumple => "success",
            EstadoCumplimiento.Parcial => "warning",
            EstadoCumplimiento.NoCumple => "danger",
            _ => "secondary"
        };

        [Display(Name = "Fecha de Evaluación")]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        [Display(Name = "Observaciones")]
        public string Observaciones { get; set; } = "";

        public int CantidadEvidencias { get; set; }
    }
}
