using InmobiliariaApp.Models;

namespace InmobiliariaApp.Services.Interfaces
{
    public interface IImagenesInmuebleService
    {
        int Alta(ImagenesInmueble entidad);
        int Baja(int id);
        int Modificacion(ImagenesInmueble entidad);
        ImagenesInmueble? ObtenerPorId(int id);
        IList<ImagenesInmueble> ObtenerLista(int paginaNro = 1, int tamPagina = 10);
        int ObtenerCantidad();
        IList<ImagenesInmueble> BuscarPorInmueble(int inmuebleiD);
        
    }
}