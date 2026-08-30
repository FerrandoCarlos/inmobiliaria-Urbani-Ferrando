using InmobiliariaApp.Common.Exceptions;
using InmobiliariaApp.Models;
using InmobiliariaApp.Repositories.Interfaces;
using InmobiliariaApp.Services.Interfaces;

namespace InmobiliariaApp.Services.Implementations
{
    public class ImagenesInmuebleService : IImagenesInmuebleService
    {
        private readonly IImagenesInmuebleRepository _repositorio;

        public ImagenesInmuebleService(IImagenesInmuebleRepository repositorio)
        {
            _repositorio = repositorio;
        }

        public IList<ImagenesInmueble> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
        {
            return _repositorio.ObtenerLista(paginaNro, tamPagina);
        }

        public int ObtenerCantidad()
        {
            return _repositorio.ObtenerCantidad();
        }

        public ImagenesInmueble? ObtenerPorId(int id)
        {
            return _repositorio.ObtenerPorId(id);
        }

        public int Alta(ImagenesInmueble entidad)
        {
            return _repositorio.Alta(entidad);
        }

        public int Modificacion(ImagenesInmueble entidad)
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

        public IList<ImagenesInmueble> BuscarPorInmueble(int inmuebleId)
        {
            return _repositorio.BuscarPorInmueble(inmuebleId);
        }
    }
}