using InmobiliariaApp.Common.Exceptions;
using InmobiliariaApp.Models;
using InmobiliariaApp.Repositories.Interfaces;
using InmobiliariaApp.Services.Interfaces;

namespace InmobiliariaApp.Services.Implementations
{
    public class ReservaFiltroService : IReservaFiltroService
    {
        private readonly IReservaFiltroRepository _repositorio;
        public ReservaFiltroService(IReservaFiltroRepository repositorio)
        {
            _repositorio = repositorio;
        }

        public IList<Reserva> ObtenerPorFiltro(ReservaFiltro filtro)
        {
            return _repositorio.ObtenerPorFiltro(filtro);
        }
    }
}