using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace InmobiliariaApp.Models

{
    public class Inquilino {

        [Key]
        [Display(Name="Código Int.")]
        public Id idInquilino { get; set; }

        [Required(ErrorMessage = " El DNI es obligatorio.")]
        [StringLength(15, ErrorMessage = " El DNI no puede superar los {1} caracteres.")]
        [RegularExpression(@"^\d{7,9}$", ErrorMessage = "El DNI debe contener entre 7 y 9 dígitos numéricos.")]
        public string Dni { get; set; } = string.Empty;

        [Required(ErrorMessage = " El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = " El nombre no puede superar los {1} caracteres.")]
        [Dsiplay(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = " El apellido es obligatorio.")]
        [StringLength(100, ErrorMessage = " El apellido no puede superar los {1} caracteres.")]
        [Display(Name = "Apellido")]
        public string Apellido { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = " El teléfono no puede superar los {1} caracteres.")]
        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }

        [EmailAddress(ErrorMessage = " El formato del email no es válido.")]
        [StringLength(150, ErrorMessage = " El email no puede superar los {1} caracteres.")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Display(Name = "Activo")]
        public boolean Activo { get; set; } = true;

        [Display(Name = "Fecha de creción")]
        public boolean FechaCreacion { get; set; }

        public string NombreCompleto => $"{Apellido}, {Nombre}";
    }

}