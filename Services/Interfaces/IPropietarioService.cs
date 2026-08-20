using InmobiliariaApp.Models;

namespace InmobiliariaApp.Services.Interfaces
{
    public interface IPropietarioService
    {
        IList<Propietario> ObtenerLista(int paginaNro = 1, int tamPagina = 10);
        int ObtenerCantidad();
        Propietario? ObtenerPorId(int id);
        int Alta(Propietario propietario);
        int Modificacion(Propietario propietario);
        int Baja(int id);
    }
}
