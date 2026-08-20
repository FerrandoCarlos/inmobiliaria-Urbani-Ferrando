using InmobiliariaApp.Models;

namespace InmobiliariaApp.Repositories.Interfaces
{
    public interface IPropietarioRepository : IRepository<Propietario>
    {
        /// Verifica si ya existe un Propietario activo o inactivo con ese DNI.
        /// idExcluir se usa al editar, para no comparar el registro contra sí mismo.
        bool ExisteDni(string dni, int idExcluir = 0);
    }
}
