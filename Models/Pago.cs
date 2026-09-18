using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace InmobiliariaApp.Models
{
    public class Pago()
    {
        [Key]
        [Display(Name = "Código Int.")]
        public int Id { get; set; }
        [Display(Name = "ID de la Reserva")]
        public int ReservaId { get; set; }
        [ForeignKey(nameof(ReservaId))]
        [BindNever]
        public Reserva? Reserva { get; set; }
        [Required(ErrorMessage = "Es necesario especificar el monto del pago.")]
        [Display(Name = "Monto del pago")]
        public decimal Monto { get; set; }
        [Required(ErrorMessage = "Es necesario especificar el concepto del pago.")]
        [Display(Name = "Concepto del pago")]
        public string Concepto { get; set; } = string.Empty;
        [Required(ErrorMessage = "Es necesario especificar el estado del pago.")]
        [Display(Name = "Estado del pago")]
        public string Estado { get; set; } = string.Empty;
        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;
        [Display(Name = "Fecha del pago")]
        public DateTime Fecha { get; set; }

        [Display(Name = "Creado por")]
        [BindNever]
        public int CreadoPorId { get; set; }

        [ForeignKey(nameof(CreadoPorId))]
        [BindNever]
        public Usuario? CreadoPor { get; set; }

        [Display(Name = "Anulado por")]
        [BindNever]
        public int? AnuladoPorId { get; set; }

        [ForeignKey(nameof(AnuladoPorId))]
        [BindNever]
        public Usuario? AnuladoPor { get; set; }
    }
}
