using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace InmobiliariaApp.Models

{
    public class Inmueble
    {
        [Key]
        [Display(Name="Código Int.")]
        public int Id { get; set; }
        
        [Display(Name = "ID del Propietario")]
        public int PropietarioId { get; set; }

        [ForeignKey(nameof(PropietarioId))]
        [BindNever]
        public Propietario? Propietario { get; set; }

        [Display(Name = "Imagen de portada")]
        public string? ImgPortadaURL { get; set; }

        // Para EF
        [NotMapped]
        public IFormFile? PortadaFile { get; set; }

        [Required(ErrorMessage = "Es necesario especificar la capacidad del inmueble")]
        [Display(Name = "Cupo")]
        public int Cupo { get; set; }

        [Required(ErrorMessage = "Es necesario especificar la dirección del inmueble")]
        [Display(Name = "Dirección")]
        public string Direccion { get; set; }

        [Required(ErrorMessage = "Es necesario especificar el tipo de inmueble")]
        [Display(Name = "Tipo de inmueble")]
        public string Tipo { get; set; }

        [Required(ErrorMessage = "Es necesario especificar la latitud del inmueble")]
        [Display(Name = "Latitud")]
        public decimal Latitud { get; set; }

        [Required(ErrorMessage = "Es necesario especificar la longitud del inmueble")]
        [Display(Name = "Longitud")]
        public decimal Longitud { get; set; }

        [Required(ErrorMessage = "Es necesario especificar el precio diario de la estadia en el inmueble")]
        [Display(Name = "Precio por día")]
        public decimal PrecioXDia { get; set; }
        
        [Display(Name = "Estado")]
        public string Estado { get; set; } = "Libre";
        
        [Required(ErrorMessage = "Es necesario especificar el porcentaje de reserva del inmueble")]
        [Display(Name = "Porcentaje de reserva")]
        public decimal PorcentajeReserva { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;

        [ForeignKey(nameof(ImagenesInmueble.InmuebleId))]
        public ICollection<ImagenesInmueble> Imagenes { get; set; } = new List<ImagenesInmueble>();
    }
}