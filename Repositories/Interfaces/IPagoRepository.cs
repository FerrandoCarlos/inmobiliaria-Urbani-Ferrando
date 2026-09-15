using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InmobiliariaApp.Models;

namespace InmobiliariaApp.Repositories.Interfaces
{
    public interface IPagoRepository : IRepository<Pago>
    {
        int ModificacionEstado(string estado, int id);
        IList<Pago> ObtenerListaPendientes(string estado, int PaginaNro, int tamPagina);
        IList<Pago> ObtenerListaMultas(int PaginaNro, int tamPagina);
        IList<Pago> ObtenerListaCancelados(int PaginaNro, int tamPagina);
        IList<Pago> ObtenerListaPagados(int PaginaNro, int tamPagina);
        Pago? BuscarPorReserva(int idReserva);
    }
}