using System.Data;
using InmobiliariaApp.Common.Exceptions;
using InmobiliariaApp.Models;
using InmobiliariaApp.Repositories.Interfaces;
using InmobiliariaApp.Services.Interfaces;

namespace InmobiliariaApp.Services.Implementations
{
    public class TipoInmuebleService : ITipoInmuebleService
    {
        private readonly ITipoInmuebleRepository _repositorio;
        public TipoInmuebleService(ITipoInmuebleRepository repositorio)
        {
            _repositorio = repositorio;
        }
        public IList<TipoInmueble> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
        {
            return _repositorio.ObtenerLista(paginaNro, tamPagina);
        }
        public int ObtenerCantidad()
        {
            return _repositorio.ObtenerCantidad();
        }
        public TipoInmueble? ObtenerPorId(int id)
        {
            return _repositorio.ObtenerPorId(id);
        }
        public int Alta(TipoInmueble tipoInmueble)
        {
            if (_repositorio.ExistePorNombre(tipoInmueble.Tipo))
            {
                throw new AppException($"Ya existe un tipo de inmueble con registrado como '{tipoInmueble.Tipo}'");
            }
            return _repositorio.Alta(tipoInmueble);
        }
        public int Modificacion(TipoInmueble tipoInmueble)
        {   
            if (_repositorio.ExistePorNombre(tipoInmueble.Tipo))
            {
                throw new AppException($"Ya existe un tipo de inmueble con registrado como '{tipoInmueble.Tipo}'");
            }
            var existente = _repositorio.ObtenerPorId(tipoInmueble.Id)
                ?? throw new AppException("El tipo de inmueble que intenta modificar no existe.");
            return _repositorio.Modificacion(tipoInmueble);
        }
        public int Baja(int id)
        {
            var existente = _repositorio.ObtenerPorId(id)
                ?? throw new AppException("El tipo de inmueble que intenta eliminar no existe");
            return _repositorio.Baja(id);
        }
    }
}