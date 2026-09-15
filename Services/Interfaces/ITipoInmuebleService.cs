using InmobiliariaApp.Models;

namespace InmobiliariaApp.Services.Interfaces
{
    public interface ITipoInmuebleService
    {
        IList<TipoInmueble> ObtenerLista(int paginaNro = 1, int tamPagina = 10);
        int ObtenerCantidad();
        TipoInmueble? ObtenerPorId(int id);
        int Alta(TipoInmueble i);
        int Modificacion(TipoInmueble i);
        int Baja(int id);
    }
}