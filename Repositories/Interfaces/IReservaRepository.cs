using InmobiliariaApp.Models;

namespace InmobiliariaApp.Repositories.Interfaces
{
    public interface IReservaRepository : IRepository<Reserva>
    {
        bool ExisteSolapamiento(int inmuebleId, DateTime fechaDesde, DateTime fechaHasta, int idExcluir = 0);
    }
}
