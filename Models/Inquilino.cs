using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace InmobiliariaApp.Models

{
    public class Inquilino
    {

        [Key]
        [Display(Name = "Código Int.")]
        public int Id { get; set; }

        [Required(ErrorMessage = " El DNI es obligatorio.")]
        [StringLength(15, ErrorMessage = " El DNI no puede superar los {1} caracteres.")]
        [RegularExpression(@"^\d{7,9}$", ErrorMessage = "El DNI debe contener entre 7 y 9 dígitos numéricos.")]
        public string Dni { get; set; } = string.Empty;

        [Required(ErrorMessage = " El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = " El nombre no puede superar los {1} caracteres.")]
        [RegularExpression(@"^[a-zA-ZÀ-ÿñÑ\s]+$", ErrorMessage = "El nombre no puede contener números ni símbolos.")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = " El apellido es obligatorio.")]
        [StringLength(100, ErrorMessage = " El apellido no puede superar los {1} caracteres.")]
        [RegularExpression(@"^[a-zA-ZÀ-ÿñÑ\s]+$", ErrorMessage = "El apellido no puede contener números ni símbolos.")]
        [Display(Name = "Apellido")]
        public string Apellido { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = " El teléfono no puede superar los {1} caracteres.")]
        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }

        [Required(ErrorMessage = " El email es obligatorio.")]
        [RegularExpression(@"^[^\s@]+@[^\s@]+\.[^\s@]+$", ErrorMessage = "El formato del email no es válido.")]
        [StringLength(150, ErrorMessage = " El email no puede superar los {1} caracteres.")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;

        [Display(Name = "Fecha de creción")]
        public DateTime FechaCreacion { get; set; }

        public string NombreCompleto => $"{Apellido}, {Nombre}";
    }

}
