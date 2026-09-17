using InmobiliariaApp.Models;

namespace InmobiliariaApp.Services.Interfaces
{
    public interface IInmuebleFiltroService
    {
        IList<Inmueble> ObtenerPorFiltro(InmuebleFiltro filtro);
    }
}