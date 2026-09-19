using System.ComponentModel.DataAnnotations;

namespace InmobiliariaApp.Models
{
    public class PerfilViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [RegularExpression(@"^[a-zA-ZÀ-ÿñÑ\s]{2,}$", ErrorMessage = "El nombre debe tener al menos 2 caracteres y no puede contener números ni símbolos.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [RegularExpression(@"^[a-zA-ZÀ-ÿñÑ\s]{2,}$", ErrorMessage = "El apellido debe tener al menos 2 caracteres y no puede contener números ni símbolos.")]
        public string Apellido { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
        public string? AvatarActual { get; set; }
    }
}
