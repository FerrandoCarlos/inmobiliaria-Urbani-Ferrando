using System;
using System.Collections.Generic;

namespace InmobiliariaApp.Models
{
    public class InmuebleFiltro
    {
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public int? SinReservasDias { get; set; }
        public string? PropietarioTexto { get; set; }
        public int? TipoInmuebleId { get; set; }
        public bool? Activo { get; set; }
        public bool MasReservadosUltimoAno { get; set; }
        public IList<Inmueble> Inmuebles { get; set; } = new List<Inmueble>();
    }
}