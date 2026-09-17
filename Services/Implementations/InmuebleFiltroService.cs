using InmobiliariaApp.Common.Exceptions;
using InmobiliariaApp.Models;
using InmobiliariaApp.Repositories.Interfaces;
using InmobiliariaApp.Services.Interfaces;

namespace InmobiliariaApp.Services.Implementations
{
    public class InmuebleFiltroService : IInmuebleFiltroService
    {
        private readonly IInmuebleFiltroRepository _repositorio;
        public InmuebleFiltroService(IInmuebleFiltroRepository repositorio)
        {
            _repositorio = repositorio;
        }
        public IList<Inmueble> ObtenerPorFiltro(InmuebleFiltro filtro)
        {
            return _repositorio.ObtenerPorFiltro(filtro);
        }
    }
}