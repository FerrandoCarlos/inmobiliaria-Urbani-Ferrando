using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InmobiliariaApp.Models;

namespace InmobiliariaApp.Repositories.Interfaces

{
    public interface IInmuebleRepository : IRepository<Inmueble>
    {
        int ModificacionEstado(string estado, int id);
        int Reactivar(int id);
        IList<Inmueble> ObtenerListaActivos(int PaginaNro, int tamPagina);
        IList<Inmueble> ObtenerListaInactivos(int PaginaNro, int tamPagina);
        int ModificarPortada(int InmuebleId, string ruta);
        IList<Inmueble> BuscarPorPropietario(int idPropietario);
        public int ObtenerCantidadInactivos();

    }
}