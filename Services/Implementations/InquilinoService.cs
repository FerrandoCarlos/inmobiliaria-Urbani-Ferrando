//using InmobiliariaApp.Common.Exceptions;
using System.Data;
using InmobiliariaApp.Common.Exceptions;
using InmobiliariaApp.Models;
using InmobiliariaApp.Repositories.Interfaces;
using InmobiliariaApp.Services.Interfaces;

namespace InmobiliariaApp.Services.Implementations
{
    public class InquilinoService : IInquilinoService
    {
        private readonly IInquilinoRepository _repositorio;

        public InquilinoService(IInquilinoRepository repositorio)
        {
            _repositorio = repositorio;
        }

        public IList<Inquilino> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
        {
            return _repositorio.ObtenerLista(paginaNro, tamPagina);
        }

        public int ObtenerCantidad()
        {
            return _repositorio.ObtenerCantidad();
        }

        public Inquilino? ObtenerPorId(int id)
        {
            return _repositorio.ObtenerPorId(id);
        }

        public int Alta(Inquilino inquilino)
        {
            ValidarDniUnico(inquilino.Dni, idExcluir: 0);

            return _repositorio.Alta(inquilino);
        }

        public int Modificacion(Inquilino inquilino)
        {
            var existente = _repositorio.ObtenerPorId(inquilino.Id)
                ?? throw new AppException("El inquilino que intenta modificar no existe.");

            ValidarDniUnico(inquilino.Dni, inquilino.Id);

            return _repositorio.Modificacion(inquilino);
        }

        public int Baja(int id)
        {
            var existente = _repositorio.ObtenerPorId(id)
                ?? throw new AppException("El inquilino que intenta eliminar no existe.");

            return _repositorio.Baja(id);
        }

        private void ValidarDniUnico(string dni, int idExcluir)
        {
            if (_repositorio.ExisteDni(dni, idExcluir))
            {
                throw new AppException($"Ya existe un inquilino registrado con el DNI {dni}.");
            }
        }

        public IList<Inquilino> ObtenerListaInactivos()
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
