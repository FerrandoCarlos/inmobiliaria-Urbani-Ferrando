using System.ComponentModel.DataAnnotations;

namespace InmobiliariaApp.Models
{
    public class Rol
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;
    }
}
