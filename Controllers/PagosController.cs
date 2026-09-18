using InmobiliariaApp.Common.Exceptions;
using InmobiliariaApp.Models;
using InmobiliariaApp.Repositories.Interfaces;
using InmobiliariaApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InmobiliariaApp.Controllers
{
    public class PagosController : Controller
    {
        private readonly IPagoService _service;
        private readonly IReservaService _reservaService;
        private const int TamPaginaDefault = 10;

        public PagosController(IPagoService service, IReservaService reservaService)
        {
            _service = service;
            _reservaService = reservaService;
        }

        //GET : /Pagos
        public IActionResult Index(int paginaNro = 1)
        {
            try
            {
                var lista = _service.ObtenerLista(paginaNro, TamPaginaDefault);
                var cantidadTotal = _service.ObtenerCantidad();

                ViewBag.PaginaNro = paginaNro;
                ViewBag.TotalPaginas = (int)Math.Ceiling(cantidadTotal / (double)TamPaginaDefault);
                return View(lista);
            }
            catch (Exception)
            {
                TempData["Error"] = "Ocurrió un error al cargar el listado de pagos.";
                return View(new List<Pago>());
            }
        }

        // GET : /Pagos/Create

        public IActionResult Create()
        {
            return View();
        }

        // GET: /Pagos/Edit/ID

        public IActionResult Edit(int id)
        {
            var pago = _service.ObtenerPorId(id);
            if (pago == null)
            {
                return NotFound();
            }
            return View(pago);
        }

        // GET: /Pagos/Details/ID
        public IActionResult Details(int id)
        {
            var pago = _service.ObtenerPorId(id);
            if (pago == null)
            {
                return NotFound();
            }
            return View(pago);
        }

        // POST: /Pagos/Guardar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Guardar([FromBody] Pago pago)
        {
            if (!ModelState.IsValid)
            {
                var errores = ModelState
                    .Where(kvp => kvp.Value?.Errors.Count > 0)
                    .SelectMany(kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage));
                return BadRequest(new { success = false, message = string.Join(" ", errores) });
            }
            try
            {
                var reservaExiste = _reservaService.ObtenerPorId(pago.ReservaId);
                if (reservaExiste == null)
                {
                    return BadRequest(new { succes = false, message = $"La Reserva con ID {pago.ReservaId} no existe." });
                }
                if (pago.Id == 0)
                {
                    var nuevoId = _service.Alta(pago);
                    return Ok(new { success = true, message = "Pago creado correctamente.", data = new { id = nuevoId } });
                }
                else
                {
                    _service.Modificacion(pago);
                    return Ok(new { success = true, message = "Pago actualizado correctamente. " });
                }
            }
            catch (AppException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "Ocurrió un error inesperado al guardar el pago" });
            }
        }

        // POST : /Pagos/Eliminar/ID
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(int id)
        {
            try
            {
                _service.Baja(id);
                return Ok(new { success = true, message = "Pago dado de baja correctamente." });
            }
            catch (AppException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "Ocurrió un error inesperado al elimianr el pago." });
            }
        }
        // POST /Pagos/Cancelar/ID
        public IActionResult Cancelar(int id)
        {
            try
            {
                _service.ModificacionEstado("Cancelado", id);
                return Ok(new { success = true, message = "Pago cancelado correctamente." });
            }
            catch (AppException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception)
            {
                return BadRequest(new { success = false, message = "Ocurrió un error inesperado. Intente de nuevo más tarde." });
            }
        }
        public IActionResult Confirmar(int id)
        {
            try
            {
                _service.ModificacionEstado("Pagado", id);
                return Ok(new { success = true, message = "Pago acreditado correctamente." });
            }
            catch (AppException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception)
            {
                return BadRequest(new { success = false, message = "Ocurrió un error inesperado. Intente de nuevo más tarde." });
            }
        }
    }
}
