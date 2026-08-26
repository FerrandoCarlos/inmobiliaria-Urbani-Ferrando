using InmobiliariaApp.Models;

namespace InmobiliariaApp.Repositories.Interfaces
{
    public interface IPropietarioRepository : IRepository<Propietario>
    {
        /// Verifica si ya existe un Propietario activo o inactivo con ese DNI.
        /// idExcluir se usa al editar, para no comparar el registro contra sí mismo.
        bool ExisteDni(string dni, int idExcluir = 0);

        /// Revierte una baja lógica: UPDATE Activo = 1
        int Reactivar(int id);

        /// Lista de registros dados de baja (Activo = 0), para la vista de Inactivos.
        IList<Propietario> ObtenerListaInactivos();

        /// Cantidad de registros inactivos, para el contador del botón en Index.
        int ObtenerCantidadInactivos();

    }
}
