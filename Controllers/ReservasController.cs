using InmobiliariaApp.Common.Exceptions;
using InmobiliariaApp.Models;
using InmobiliariaApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InmobiliariaApp.Controllers
{
    public class ReservasController : Controller
    {
        private readonly IReservaService _service;
        private readonly IInquilinoService _inquilinoService;
        private readonly IInmuebleService _inmuebleService;
        private readonly ILogger<ReservasController> _logger;
        private const int tamPaginaDefault = 10;

        public ReservasController(IReservaService service, IInquilinoService inquilinoService, IInmuebleService inmuebleService, ILogger<ReservasController> logger)
        {
            _service = service;
            _inquilinoService = inquilinoService;
            _inmuebleService = inmuebleService;
            _logger = logger;
        }

        // GET: /Reservas
        public IActionResult Index(int paginaNro = 1)
        {
            try
            {
                var lista = _service.ObtenerLista(paginaNro, tamPaginaDefault);
                var cantidadTotal = _service.ObtenerCantidad();

                ViewBag.PaginaNro = paginaNro;
                ViewBag.TotalPaginas = (int)Math.Ceiling(cantidadTotal / (double)tamPaginaDefault);

                return View(lista);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar el listado de reservas.");
                TempData["Error"] = "Ocurrió un error al cargar el listado de reservas.";
                return View(new List<Reserva>());
            }
        }

        // GET: /Reservas/ObtenerFechasOcupadas/ID
        public IActionResult ObtenerFechasOcupadas(int inmuebleId)
        {
            var reservas = _service.ObtenerPorInmueble(inmuebleId);
            var rangosOcupados = reservas.Select(r => new
            {
                desde = r.FechaDesde.ToString("yyyy-MM-dd"),
                hasta = r.FechaHasta.ToString("yyyy-MM-dd")
            });
            return Ok(rangosOcupados);
        }

        // GET: /Reservas/Details/ID
        public IActionResult Details(int id)
        {
            var reserva = _service.ObtenerPorId(id);

            if (reserva == null)
            {
                return NotFound();
            }
            return View(reserva);
        }

        // GET: /Reservas/Create
        public IActionResult Create(int? reservaOriginalId = null)
        {
            var nuevaReserva = new Reserva();
            if (reservaOriginalId.HasValue && reservaOriginalId > 0)
            {
                var reservaAnterior = _service.ObtenerPorId(reservaOriginalId.Value);
                if (reservaAnterior != null)
                {
                    nuevaReserva.InquilinoId = reservaAnterior.InquilinoId;
                    nuevaReserva.InmuebleId = reservaAnterior.InmuebleId;
                    nuevaReserva.FechaDesde = reservaAnterior.FechaHasta.AddDays(1);
                    nuevaReserva.FechaHasta = reservaAnterior.FechaHasta.AddDays(2);
                }
            }
            CargarListasParaSelects();
            return View(nuevaReserva);
        }

        // GET: /Reservas/Edit/ID
        public IActionResult Edit(int id)
        {
            var reserva = _service.ObtenerPorId(id);
            if (reserva == null)
            {
                return NotFound();
            }
            CargarListasParaSelects();
            return View(reserva);
        }

        // POST: /Reservas/Guardar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Guardar([FromBody] Reserva reserva)
        {
            if (!ModelState.IsValid)
            {
                var errores = ModelState
                    .Where(kvp => kvp.Value?.Errors.Count > 0)
                    .SelectMany(kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage));

                return BadRequest(new { success = false, message = string.Join("", errores) });
            }

            try
            {
                if (reserva.Id == 0)
                {
                    var nuevoId = _service.Alta(reserva);
                    return Ok(new { success = true, message = "Reserva creada correctamente.", data = new { id = nuevoId } });
                }
                else
                {
                    _service.Modificacion(reserva);
                    return Ok(new { success = true, message = "Reserva actualizada correctamente." });
                }
            }
            catch (AppException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar la reserva.");
                return StatusCode(500, new { success = false, message = "Ocurrió un error inesperado al guardar la reserva." });
            }
        }
        // POST: /Reservas/Eliminar/ID
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(int id)
        {
            try
            {
                _service.Baja(id);
                return Ok(new { success = true, message = "Reserva finalizada correctamente." });
            }
            catch (AppException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al finalizar la reserva.");
                return StatusCode(500, new { success = false, message = "Ocurrió un error inesperado al finalizar la reserva." });
            }
        }

        // POST: /Reservas/Finalizar/ID
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Finalizar(int id)
        {
            try
            {
                _service.Finalizar(id);
                return Ok(new { success = true, message = "Reserva finalizada correctamente." });
            }
            catch (AppException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al finalizar la reserva.");
                return StatusCode(500, new { success = false, message = "Ocurrió un error inesperado al finalizar la reserva." });
            }
        }

        private void CargarListasParaSelects()
        {
            ViewBag.Inquilinos = _inquilinoService.ObtenerLista(1, 1000);
            ViewBag.Inmuebles = _inmuebleService.ObtenerListaActivos(1, 1000);
        }
    }
}
