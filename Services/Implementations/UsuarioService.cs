using InmobiliariaApp.Common.Exceptions;
using InmobiliariaApp.Models;
using InmobiliariaApp.Repositories.Interfaces;
using InmobiliariaApp.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace InmobiliariaApp.Services.Implementations
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _repositorio;
        private readonly PasswordHasher<Usuario> _hasher = new();

        public UsuarioService(IUsuarioRepository repositorio)
        {
            _repositorio = repositorio;
        }

        public IList<Usuario> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
        {
            return _repositorio.ObtenerLista(paginaNro, tamPagina);
        }

        public int ObtenerCantidad()
        {
            return _repositorio.ObtenerCantidad();
        }

        public Usuario? ObtenerPorId(int id)
        {
            return _repositorio.ObtenerPorId(id);
        }

        public int Alta(Usuario usuario, string passwordPlano)
        {
            if (_repositorio.ExisteEmail(usuario.Email))
            {
                throw new AppException($"Ya existe un usuario registrado con el email {usuario.Email}.");
            }

            if (string.IsNullOrWhiteSpace(passwordPlano) || passwordPlano.Length < 6)
            {
                throw new AppException("La contraseña debe tener al menos 6 caracteres.");
            }

            usuario.PasswordHash = _hasher.HashPassword(usuario, passwordPlano);

            return _repositorio.Alta(usuario);
        }

        public int Modificacion(Usuario usuario)
        {
            var existente = _repositorio.ObtenerPorId(usuario.Id)
                ?? throw new AppException("El usuario que intenta modificar no existe.");

            if (_repositorio.ExisteEmail(usuario.Email, usuario.Id))
            {
                throw new AppException($"Ya existe otro usuario registrado con el email {usuario.Email}.");
            }

            return _repositorio.Modificacion(usuario);
        }

        public int Baja(int id)
        {
            var existente = _repositorio.ObtenerPorId(id)
                ?? throw new AppException("El usuario que intenta eliminar no existe.");
            return _repositorio.Baja(id);
        }

        public Usuario? ValidarCredenciales(string email, string passwordPlano)
        {
            var usuario = _repositorio.ObtenerPorEmail(email);
            if (usuario == null) return null;

            var resultado = _hasher.VerifyHashedPassword(usuario, usuario.PasswordHash, passwordPlano);
            if (resultado == PasswordVerificationResult.Failed) return null;

            return usuario;
        }

        public void CambiarPasswordPropio(int id, string passwordActual, string passwordNueva)
        {
            var usuario = _repositorio.ObtenerPorId(id)
                ?? throw new AppException("El usuario no existe.");
            var resultado = _hasher.VerifyHashedPassword(usuario, usuario.PasswordHash, passwordActual);
            if (resultado == PasswordVerificationResult.Failed)
            {
                throw new AppException("La contraseña actual no es correcta.");
            }

            if (string.IsNullOrWhiteSpace(passwordNueva) || passwordNueva.Length < 6)
            {
                throw new AppException("La nueva contraseña debe tener al menos 6 caracteres.");
            }

            var nuevoHash = _hasher.HashPassword(usuario, passwordNueva);
            _repositorio.CambiarPassword(id, nuevoHash);
        }

        public void ResetearPassword(int id, string passwordNueva)
        {
            var usuario = _repositorio.ObtenerPorId(id)
                ?? throw new AppException("El usuario no existe.");

            if (string.IsNullOrWhiteSpace(passwordNueva) || passwordNueva.Length < 6)
            {
                throw new AppException("La nueva contraseña debe tener al menos 6 caracteres.");
            }

            var nuevoHash = _hasher.HashPassword(usuario, passwordNueva);
            _repositorio.CambiarPassword(id, nuevoHash);
        }
    }
}
