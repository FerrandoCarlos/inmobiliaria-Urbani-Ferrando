using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InmobiliariaApp.Models;

namespace InmobiliariaApp.Repositories.Interfaces

{
    public interface IInmuebleRepository : IRepository<Inmueble>
    {
        int ModificarPortada(int InmuebleId, string ruta);
        IList<Inmueble> BuscarPorPropietario(int idPropietario);
    }
}