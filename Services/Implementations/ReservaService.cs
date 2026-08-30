using InmobiliariaApp.Common.Exceptions;
using InmobiliariaApp.Models;
using InmobiliariaApp.Repositories.Interfaces;
using InmobiliariaApp.Services.Interfaces;

namespace InmobiliariaApp.Services.Implementations
{
    public class ReservaService : IReservaService
    {
        private readonly IReservaRepository _repositorio;
        private readonly IInmuebleRepository _inmuebleRepositorio;

        public ReservaService(IReservaRepository repositorio, IInmuebleRepository inmuebleRepositorio)
        {
            _repositorio = repositorio;
            _inmuebleRepositorio = inmuebleRepositorio;
        }

        public IList<Reserva> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
        {
            return _repositorio.ObtenerLista(paginaNro, tamPagina);
        }

        public int ObtenerCantidad()
        {
            return _repositorio.ObtenerCantidad();
        }

        public Reserva? ObtenerPorId(int id)
        {
            return _repositorio.ObtenerPorId(id);
        }

        public int Alta(Reserva reserva)
        {
            ValidarFechas(reserva.FechaDesde, reserva.FechaHasta);

            var inmueble = _inmuebleRepositorio.ObtenerPorId(reserva.InmuebleId)
                ?? throw new AppException("El inmueble seleccionado no existe.");

            if (!inmueble.Activo)
            {
                throw new AppException("El inmueble no está disponible para alquilar.");
            }

            ValidarSolapamiento(reserva.InmuebleId, reserva.FechaDesde, reserva.FechaHasta, idExcluir: 0);

            reserva.MontoPorDia = inmueble.PrecioXDia;
            reserva.Estado = "Vigente";

            return _repositorio.Alta(reserva);
        }
        public int Modificacion(Reserva reserva)
        {
            var existente = _repositorio.ObtenerPorId(reserva.Id)
                ?? throw new AppException("La reserva que intenta modificar no existe.");


            ValidarFechas(reserva.FechaDesde, reserva.FechaHasta);
            ValidarSolapamiento(reserva.InmuebleId, reserva.FechaDesde, reserva.FechaHasta, idExcluir: 0);


            var inmueble = _inmuebleRepositorio.ObtenerPorId(reserva.InmuebleId)
                ?? throw new AppException("El inmueble seleccionado no existe.");

            reserva.MontoPorDia = inmueble.PrecioXDia;


            return _repositorio.Modificacion(reserva);
        }
        public int Baja(int id)
        {

            var existente = _repositorio.ObtenerPorId(id)
                ?? throw new AppException("La reserva que intenta modificar no existe.");

            return _repositorio.Baja(id);
        }

        private static void ValidarFechas(DateTime fechaDesde, DateTime fechaHasta)
        {
            if (fechaHasta <= fechaDesde)
            {
                throw new AppException("La fecha hasta debe ser posterior a la fecha desde.");
            }
        }

        private void ValidarSolapamiento(int inmuebleId, DateTime fechaDesde, DateTime fechaHasta, int idExcluir)
        {
            if (_repositorio.ExisteSolapamiento(inmuebleId, fechaDesde, fechaHasta, idExcluir))
            {
                throw new AppException("El inmueble ya tiene una reserva vigente que se superpone con esas fechas.");
            }
        }
    }
}
