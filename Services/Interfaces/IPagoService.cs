using InmobiliariaApp.Models;

namespace InmobiliariaApp.Services.Interfaces
{
    public interface IPagoService
    {
        int Alta(Pago entidad);
        int Baja(int id);
        int ModificacionEstado(string estado, int id);
        int Modificacion(Pago entidad);
        Pago? ObtenerPorId(int id);
        IList<Pago> ObtenerListaPendientes(string estado, int PaginaNro, int tamPagina);
        IList<Pago> ObtenerListaMultas(int PaginaNro, int tamPagina);
        IList<Pago> ObtenerListaCancelados(int PaginaNro, int tamPagina);
        IList<Pago> ObtenerListaPagados(int PaginaNro, int tamPagina);
        IList<Pago> ObtenerLista(int PaginaNro, int tamPagina);
        Pago? BuscarPorReserva(int idReserva);
        int ObtenerCantidad();
        
    }
}