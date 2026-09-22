using InmobiliariaApp.Models;

namespace InmobiliariaApp.Services.Interfaces
{
    public interface IReservaFiltroService
    {
        IList<Reserva> ObtenerPorFiltro(ReservaFiltro filtro);
    }
}