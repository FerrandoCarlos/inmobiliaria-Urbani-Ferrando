using System.Collections.Generic;

namespace InmobiliariaApp.Repositories.Interfaces
{
    public interface IRepository<T>
    {
        int Alta(T entidad);
        int Baja(int id);
        int Modificacion(T entidad);

        IList<T> ObtenerLista(int paginaNro = 1, int tamPagina = 10);

        int ObtenerCantidad();

        T? ObtenerPorId(int id);
    }
}
