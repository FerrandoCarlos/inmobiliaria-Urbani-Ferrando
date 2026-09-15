using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace InmobiliariaApp.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El email es obligatorio.")]
        [RegularExpression(@"^[^\s@]+@[^\s@]+\.[^\s@]+$", ErrorMessage = "El formato del email no es válido.")]
        public string Email { get; set; } = string.Empty;

        [BindNever]
        public string PasswordHash { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los {1} caracteres.")]
        [RegularExpression(@"^[a-zA-ZÀ-ÿñÑ\s]{2,}$", ErrorMessage = "El nombre debe tener al menos 2 caracteres y no puede contener números ni símbolos.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(100, ErrorMessage = "El apellido no puede superar los {1} caracteres.")]
        [RegularExpression(@"^[a-zA-ZÀ-ÿñÑ\s]{2,}$", ErrorMessage = "El apellido debe tener al menos 2 caracteres y no puede contener números ni símbolos.")]
        public string Apellido { get; set; } = string.Empty;

        public string? Avatar { get; set; }

        [Required]
        [Display(Name = "Rol")]
        public int RolId { get; set; }

        [ForeignKey(nameof(RolId))]
        [BindNever]
        public Rol? Rol { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; }

        [NotMapped]
        public string? Password { get; set; }

    }
}
