using InmobiliariaApp.Models;

namespace InmobiliariaApp.Services.Interfaces
{
    public interface IInquilinoService
    {
        IList<Inquilino> ObtenerLista(int paginaNro = 1, int tamPagina = 10);
        int ObtenerCantidad();

        Inquilino? ObtenerPorId(int id);

        int Alta(Inquilino i);

        int Modificacion(Inquilino i);

        int Baja(int id);

        IList<Inquilino> ObtenerListaInactivos();
        int ObtenerCantidadInactivos();
        int Reactivar(int id);

    }
}
