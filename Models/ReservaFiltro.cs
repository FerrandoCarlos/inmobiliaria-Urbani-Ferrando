namespace InmobiliariaApp.Models
{
    public class ReservaFiltro
    {
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }

        public int? FinalizanEnDias { get; set; }
    }
}