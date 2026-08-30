using InmobiliariaApp.Models;

namespace InmobiliariaApp.Services.Interfaces
{
    public interface IReservaService
    {
        IList<Reserva> ObtenerLista(int paginaNro = 1, int tamPagina = 10);
        int ObtenerCantidad();
        Reserva? ObtenerPorId(int id);
        int Alta(Reserva reserva);
        int Modificacion(Reserva reserva);
        int Baja(int id);
    }
}
