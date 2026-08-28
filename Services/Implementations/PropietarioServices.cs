using InmobiliariaApp.Common.Exceptions;
using InmobiliariaApp.Models;
using InmobiliariaApp.Repositories.Interfaces;
using InmobiliariaApp.Services.Interfaces;

namespace InmobiliariaApp.Services.Implementations
{
    public class PropietarioService : IPropietarioService
    {
        private readonly IPropietarioRepository _repositorio;

        public PropietarioService(IPropietarioRepository repositorio)
        {
            _repositorio = repositorio;
        }

        public IList<Propietario> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
        {
            return _repositorio.ObtenerLista(paginaNro, tamPagina);
        }

        public int ObtenerCantidad()
        {
            return _repositorio.ObtenerCantidad();
        }

        public Propietario? ObtenerPorId(int id)
        {
            return _repositorio.ObtenerPorId(id);
        }

        public int Alta(Propietario propietario)
        {
            ValidarDniUnico(propietario.Dni, idExcluir: 0);

            return _repositorio.Alta(propietario);
        }

        public int Modificacion(Propietario propietario)
        {
            var existente = _repositorio.ObtenerPorId(propietario.Id)
                ?? throw new AppException("El propietario que intenta modificar no existe.");

            ValidarDniUnico(propietario.Dni, propietario.Id);

            return _repositorio.Modificacion(propietario);
        }

        public int Baja(int id)
        {
            var existente = _repositorio.ObtenerPorId(id)
                ?? throw new AppException("El propietario que intenta eliminar no existe.");

            return _repositorio.Baja(id);
        }

        private void ValidarDniUnico(string dni, int idExcluir)
        {
            if (_repositorio.ExisteDni(dni, idExcluir))
            {
                throw new AppException($"Ya existe un propietario registrado con el DNI {dni}.");
            }
        }

        public IList<Propietario> ObtenerListaInactivos()
        {
            return _repositorio.ObtenerListaInactivos();
        }

        public int ObtenerCantidadInactivos()
        {
            return _repositorio.ObtenerCantidadInactivos();
        }

        public int Reactivar(int id)
        {
            var existente = _repositorio.ObtenerPorId(id)
                ?? throw new AppException("El propietario que intenta reactivar no existe.");

            return _repositorio.Reactivar(id);
        }
    }
}
