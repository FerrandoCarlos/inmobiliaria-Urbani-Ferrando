using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InmobiliariaApp.Models;

namespace InmobiliariaApp.Repositories.Interfaces
{
    public interface IImagenesInmuebleRepository : IRepository<ImagenesInmueble>
    {
        IList<ImagenesInmueble> BuscarPorInmueble(int inmuebleId);
    }
}