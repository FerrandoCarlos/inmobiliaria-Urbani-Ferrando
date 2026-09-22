using InmobiliariaApp.Models;

namespace InmobiliariaApp.Repositories.Interfaces
{
    public interface IUsuarioRepository : IRepository<Usuario>
    {
        Usuario? ObtenerPorEmail(string email);
        bool ExisteEmail(string email, int idExcluir = 0);
        int CambiarPassword(int id, string nuevoHash);

        int ActualizarPerfil(int id, string nombre, string apellido, string? avatarUrl);

        int EliminarAvatar(int id);
    }
}
