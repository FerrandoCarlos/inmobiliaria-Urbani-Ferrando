using InmobiliariaApp.Common.Exceptions;
using InmobiliariaApp.Models;
using InmobiliariaApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InmobiliariaApp.Controllers
{
    public class ReservasController : Controller
    {
        private readonly IReservaService _service;
        private const int tamPaginaDefault = 10;

        public ReservasController(IReservaService service)
        {
            _service = service;
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
            catch (Exception)
            {
                TempData["Error"] = "Ocurrió un error al cargar el listado de reservas.";
                return View(new List<Reserva>());
            }
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
        public IActionResult Create()
        {
            return View();
        }

        // GET: /Reservas/Edit/ID
        public IActionResult Edit(int id)
        {
            var reserva = _service.ObtenerPorId(id);
            if (reserva == null)
            {
                return NotFound();
            }
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

                return BadRequest(new { succes = false, message = string.Join("", errores) });
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
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "Ocurrió un error inesperado al guardar la reserva." });
            }
        }
        // POST: /Reservas/Eliminar/ID

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
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "Ocurrió un error inesperado al finalizar la reserva." });
            }
        }
    }
}
