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
        private readonly IPagoRepository _pagoRepositorio;

        public ReservaService(IReservaRepository repositorio, IInmuebleRepository inmuebleRepositorio, IPagoRepository pagoRepositorio)
        {
            _repositorio = repositorio;
            _inmuebleRepositorio = inmuebleRepositorio;
            _pagoRepositorio = pagoRepositorio;
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
        public IList<Reserva> ObtenerPorInmueble(int id)
        {
            return _repositorio.ObtenerPorInmueble(id);
        }

        public int Finalizar(int id)
        {
            var reserva = _repositorio.ObtenerPorId(id);
            if (reserva == null)
            {
                throw new AppException("La reserva especificada no existe.");
            }

            if (reserva.Estado == "Finalizado")
            {
                throw new AppException("La reserva ya se encuentra finalizada.");
            }

            DateTime fechaTerminacion = DateTime.Now;

            if (fechaTerminacion.Date < reserva.FechaHasta.Date)
            {
                int diasTotales = (reserva.FechaHasta - reserva.FechaDesde).Days;
                if (diasTotales <= 0) diasTotales = 1;
                int diasOcupados = (fechaTerminacion.Date - reserva.FechaDesde.Date).Days;
                if (diasOcupados < 1) diasOcupados = 1;
                int diasRestantes = (reserva.FechaHasta.Date - fechaTerminacion.Date).Days;
                if (diasRestantes > 0)
                {
                    decimal porcentajePenalizacion = (diasOcupados < (diasTotales / 2.0)) ? 0.50m: 0.25m;
                    decimal costoDiasOcupados = diasOcupados * reserva.MontoPorDia;
                    decimal costoDiasRestantes = diasRestantes * reserva.MontoPorDia;
                    decimal montoMulta = costoDiasOcupados + (costoDiasRestantes * porcentajePenalizacion);

                    montoMulta = Math.Round(montoMulta, 2);

                    reserva.Multa = montoMulta;
                    var pagoMulta = new Pago
                    {
                        ReservaId = id,
                        Monto = montoMulta,
                        Concepto = "Multa",
                        Fecha = fechaTerminacion,
                        Estado = "Pendiente"
                    };
                    _pagoRepositorio.Alta(pagoMulta);
                    _pagoRepositorio.CancelarSaldoRestantePendiente(id);
                }
            }

            return _repositorio.Finalizar(reserva);
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

            int reservaId= _repositorio.Alta(reserva);

            decimal porcentajeReserva = inmueble?.PorcentajeReserva ?? 30m;
            int cantidadDias = (reserva.FechaHasta - reserva.FechaDesde).Days;
            if (cantidadDias <= 0) cantidadDias = 1;

            decimal montoTotal = cantidadDias * reserva.MontoPorDia;
            decimal montoSeña = Math.Round(montoTotal * (porcentajeReserva / 100m), 2);
            decimal montoSaldo = montoTotal - montoSeña;

            var pagoSeña = new Pago
            {
                ReservaId = reservaId,
                Monto = montoSeña,
                Concepto = "Porcentaje Reserva",
                Fecha = DateTime.Now,
                Estado = "Pendiente"
            };
            _pagoRepositorio.Alta(pagoSeña);

            var pagoSaldo = new Pago
            {
                ReservaId = reservaId,
                Monto = montoSaldo,
                Concepto = "Saldo Restante",
                Fecha = DateTime.Now,
                Estado = "Pendiente"
            };
            _pagoRepositorio.Alta(pagoSaldo);
            
            return reservaId;
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
