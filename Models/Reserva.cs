using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace InmobiliariaApp.Models
{
    public class Reserva
    {
        [Key]
        [Display(Name = "Código Int.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Es necesario especificar el inquilino.")]
        [Display(Name = "Inquilino")]
        public int InquilinoId { get; set; }

        [ForeignKey(nameof(InquilinoId))]
        [BindNever]
        public Inquilino? Inquilino { get; set; }

        [Required(ErrorMessage = "Es necesario especificar el inmueble.")]
        [Display(Name = "Inmueble")]
        public int InmuebleId { get; set; }

        [ForeignKey(nameof(InmuebleId))]
        [BindNever]
        public Inmueble? Inmueble { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
        [Display(Name = "Fecha desde")]
        [DataType(DataType.Date)]
        public DateTime FechaDesde { get; set; }

        [Required(ErrorMessage = "La fecha de fin es obligatoria.")]
        [Display(Name = "Fecha hasta")]
        [DataType(DataType.Date)]
        public DateTime FechaHasta { get; set; }

        [Display(Name = "Fecha de terminación anticipada")]
        [DataType(DataType.Date)]

        public DateTime? FechaTerminacion { get; set; }

        [Required(ErrorMessage = "El monto por día es obligatorio.")]
        [Display(Name = "Monto por día")]
        public decimal MontoPorDia { get; set; }

        [Display(Name = "Multa")]
        public decimal? Multa { get; set; }

        [Display(Name = "Estado")]
        public string Estado { get; set; } = "Vigente";

        [Display(Name = "Fecha de creación")]
        public DateTime FechaCreacion { get; set; }
    }
}
