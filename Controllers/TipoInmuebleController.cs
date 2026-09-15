using System.Security.Cryptography;
using InmobiliariaApp.Common.Exceptions;
using InmobiliariaApp.Models;
using InmobiliariaApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InmobiliariaApp.Controllers
{
    public class TipoInmuebleController : Controller
    {
        private readonly ITipoInmuebleService _service;
        ILogger<TipoInmuebleController> _logger;
        private const int tamPaginaDefault = 10;
        public TipoInmuebleController(ITipoInmuebleService service, ILogger<TipoInmuebleController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // GET: /TipoInmueble
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
                _logger.LogError(ex, "Error al cargar el listado de tipo inmueble.");
                TempData["Error"] = "Ocurrió un error al cargar el listado de tipos de inmuebles.";
                return View(new List<TipoInmueble>());
            }
        }

        // GET : /TipoInmueble/Create
        public IActionResult Create()
        {
            return View();
        }

        // GET : /TipoInmueble/Edit/ID
        public IActionResult Edit(int id)
        {
            var tipoInmueble = _service.ObtenerPorId(id);
            if (tipoInmueble == null)
            {
                return NotFound();
            }
            return View(tipoInmueble);
        }

        // POST : /TipoInmueble/Guardar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Guardar([FromBody] TipoInmueble tipoInmueble)
        {
            if (!ModelState.IsValid)
            {
                var errores = ModelState
                    .Where(kvp => kvp.Value?.Errors.Count > 0)
                    .SelectMany(kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage));
                return BadRequest(new { success = false, message = string.Join(" ", errores)});
            }
            try
            {
                if (tipoInmueble.Id == 0)
                {
                    var nuevoId = _service.Alta(tipoInmueble);
                    return Ok(new { success = true, message = "Tipo de inmueble creado correctamente.", data = new {id = nuevoId}});
                }
                else
                {
                    _service.Modificacion(tipoInmueble);
                    return Ok(new { success = true, message = "Tipo de inmueble actualizado correctamente."});
                }
            }
            catch (AppException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al guardar el tipo de inmueble.");
                return StatusCode(500, new { success = false, message = "Ocurrió un error inesperado al guardar el tipo de inmueble."});
            }
        }

        // POST /TipoInmueble/Eliminar/ID
        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Eliminar(int id)
        {
            try
            {
                _service.Baja(id);
                return Ok(new { success = true, message = "Tipo de inmueble dado de baja correctamente. "});
            }
            catch (AppException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar el tipo de inmueble.");
                return StatusCode(500, new { success = false, message = "Ocurrió un error inesperado al eliminar el tipo de inmueble."});
            }
        }
    }
}