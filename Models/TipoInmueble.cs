using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace InmobiliariaApp.Models
{
    public class TipoInmueble()
    {
        [Key]
        [Display(Name = "Código Int.")]
        public int Id { get; set; }
        [Display(Name = "Tipo del inmueble")]
        public string Tipo { get; set; }
        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;
    }
}