using InmobiliariaApp.Common.Exceptions;
using InmobiliariaApp.Models;
using InmobiliariaApp.Repositories.Interfaces;
using InmobiliariaApp.Services.Interfaces;

namespace InmobiliariaApp.Services.Implementations
{
    public class PagoService : IPagoService
    {
        private readonly IPagoRepository _repositorio;

        public PagoService(IPagoRepository repositorio)
        {
            _repositorio = repositorio;
        }

        public int Alta (Pago entidad)
        {
            return _repositorio.Alta(entidad);
        }

        public int Baja (int id)
        {
            return _repositorio.Baja(id);
        }

        public int ModificacionEstado (string estado, int id)
        {
            var existente = _repositorio.ObtenerPorId(id)
                ?? throw new AppException("El pago que intenta modificar no existe.");
            return _repositorio.ModificacionEstado(estado, id);
        }

        public int Modificacion(Pago entidad)
        {
            var existente = _repositorio.ObtenerPorId(entidad.Id)
                ?? throw new AppException("El pago que intenta modificar no existe.");
            return _repositorio.Modificacion(entidad);
        }
        
        public Pago? ObtenerPorId(int id)
        {
            return _repositorio.ObtenerPorId(id);
        }

        public IList<Pago> ObtenerListaPendientes(string estado, int PaginaNro, int tamPagina)
        {
            return _repositorio.ObtenerListaPendientes(estado, PaginaNro, tamPagina);
        }

        public IList<Pago> ObtenerListaMultas(int PaginaNro, int tamPagina)
        {
            return _repositorio.ObtenerListaMultas(PaginaNro, tamPagina);
        }

        public IList<Pago> ObtenerListaCancelados(int PaginaNro, int tamPagina)
        {
            return _repositorio.ObtenerListaCancelados(PaginaNro, tamPagina);
        }

        public IList<Pago> ObtenerListaPagados(int PaginaNro, int tamPagina)
        {
            return _repositorio.ObtenerListaPagados(PaginaNro, tamPagina);
        }

        public IList<Pago> ObtenerLista(int PaginaNro, int tamPagina)
        {
            return _repositorio.ObtenerLista(PaginaNro, tamPagina);
        }

        public Pago? BuscarPorReserva(int idReserva)
        {   
            var existente = _repositorio.BuscarPorReserva(idReserva)
                ?? throw new AppException("No existe la reserva.");
            return existente;
        }

        public int ObtenerCantidad()
        {
            return _repositorio.ObtenerCantidad();
        }

    }
}