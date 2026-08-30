using InmobiliariaApp.Common.Exceptions;
using InmobiliariaApp.Models;
using InmobiliariaApp.Repositories.Interfaces;
using InmobiliariaApp.Services.Interfaces;

namespace InmobiliariaApp.Services.Implementations
{
    public class InmuebleService : IInmuebleService
    {
        private readonly IInmuebleRepository _repositorio;

        public InmuebleService(IInmuebleRepository repositorio)
        {
            _repositorio = repositorio;
        }

        public IList<Inmueble> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
        {
            return _repositorio.ObtenerLista(paginaNro, tamPagina);
        }

        public int ObtenerCantidad()
        {
            return _repositorio.ObtenerCantidad();
        }

        public Inmueble? ObtenerPorId(int id)
        {
            return _repositorio.ObtenerPorId(id);
        }

        public int Alta(Inmueble entidad)
        {
            return _repositorio.Alta(entidad);
        }

        public int Modificacion(Inmueble entidad)
        {
            var existente = _repositorio.ObtenerPorId(entidad.Id)
                ?? throw new AppException("El inmueble que intenta modificar no existe.");
            
            return _repositorio.Modificacion(entidad);
        }

        public int Baja(int id)
        {
            var existente = _repositorio.ObtenerPorId(id)
                ?? throw new AppException("El inmueble que intenta eliminar no existe.");

            return _repositorio.Baja(id);
        }

        public IList<Inmueble> ObtenerListaActivos(int paginaNro = 1, int tamPagina = 10)
        {
            return _repositorio.ObtenerListaActivos(paginaNro, tamPagina);
        }

        public IList<Inmueble> ObtenerListaInactivos(int paginaNro = 1, int tamPagina = 10)
        {
            return _repositorio.ObtenerListaInactivos(paginaNro, tamPagina);
        }

        public int Reactivar(int id)
        {
            var existente = _repositorio.ObtenerPorId(id)
                ?? throw new AppException("El inmueble que intenta reactivar no existe.");
            
            return _repositorio.Reactivar(id);
        }

        public int ModificacionEstado(string estado, int id)
        {
            var existente = _repositorio.ObtenerPorId(id)
                ?? throw new AppException("El inmueble que intenta modificar no existe.");

            return _repositorio.ModificacionEstado(estado, id);
        }

        public int ModificarPortada(int id, string url)
        {
            var existente = _repositorio.ObtenerPorId(id)
                ?? throw new AppException("El inmueble que intenta modificar no existe.");

            return _repositorio.ModificarPortada(id, url); 
        }

        public int ObtenerCantidadInactivos()
        {
            return _repositorio.ObtenerCantidadInactivos();
        }

        public IList<Inmueble> BuscarPorPropietario(int idPropietario)
        {
            return _repositorio.BuscarPorPropietario(idPropietario);
        }
    }
}