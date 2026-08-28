using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InmobiliariaApp.Models;

namespace InmobiliariaApp.Repositories.Interfaces
{
    public interface IInquilinoRepository : IRepository<Inquilino>
    {
        bool ExisteDni(string dni, int idExcluir = 0);

        /// Revierte una baja lógica: UPDATE Activo = 1
        int Reactivar(int id);

        /// Lista de registros dados de baja (Activo = 0), para la vista de Inactivos.
        IList<Inquilino> ObtenerListaInactivos();

        /// Cantidad de registros inactivos, para el contador del botón en Index.
        int ObtenerCantidadInactivos();
    }
}
