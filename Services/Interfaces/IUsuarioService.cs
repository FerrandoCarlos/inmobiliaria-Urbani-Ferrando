using InmobiliariaApp.Models;

namespace InmobiliariaApp.Services.Interfaces
{
    public interface IUsuarioService
    {
        IList<Usuario> ObtenerLista(int paginaNro = 1, int tamPagina = 10);
        int ObtenerCantidad();
        Usuario? ObtenerPorId(int id);
        int Alta(Usuario usuario, string passwordPlano);
        int Modificacion(Usuario usuario);
        int Baja(int id);

        // Login: null si el email no existe, está inactivo, o la contraseña no coincide.
        Usuario? ValidarCredenciales(string email, string passwordPlano);

        // Autogestión de perfil: exige la contraseña actual antes de cambiarla.
        void CambiarPasswordPropio(int id, string passwordActual, string passwordNueva);

        // Reseteo por un Administrador: no exige la contraseña anterior.
        void ResetearPassword(int id, string passwordNueva);

        void ActualizarPerfil(int id, string nombre, string apellido, string? nuevoAvatarUrl);
    }
}
