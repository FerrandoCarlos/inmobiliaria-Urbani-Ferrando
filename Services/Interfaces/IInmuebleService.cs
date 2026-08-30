using InmobiliariaApp.Models;

namespace InmobiliariaApp.Services.Interfaces
{
    public interface IInmuebleService
    {
        IList<Inmueble> ObtenerLista(int paginaNro = 1, int tamPagina = 10);
        int ObtenerCantidad();
        Inmueble? ObtenerPorId(int id);
        int Alta(Inmueble entidad);
        int Modificacion(Inmueble entidad);
        int Baja(int id);
        IList<Inmueble> ObtenerListaActivos(int paginaNro = 1, int tamPagina = 10);
        IList<Inmueble> ObtenerListaInactivos(int paginaNro = 1, int tamPagina = 10);
        int Reactivar(int id);
        int ModificacionEstado(string estado, int id);
        int ModificarPortada(int id, string url);
        int ObtenerCantidadInactivos();
        IList<Inmueble> BuscarPorPropietario(int idPropietario);
    }
}