using InmobiliariaApp.Common.Exceptions;
using InmobiliariaApp.Models;
using InmobiliariaApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InmobiliariaApp.Controllers
{
    public class InquilinosController : Controller
    {
        private readonly IInquilinoService _service;
        private const int TamPaginaDefault = 10;

        public InquilinosController(IInquilinoService service)
        {
            _service = service;
        }

        // GET : /Inquilinos
        public IActionResult Index(int paginaNro = 1)
        {
            try
            {
                var lista = _service.ObtenerLista(paginaNro, TamPaginaDefault);
                var cantidadTotal = _service.ObtenerCantidad();

                ViewBag.PaginaNro = paginaNro;
                ViewBag.TotalPaginas = (int)Math.Ceiling(cantidadTotal / (double)TamPaginaDefault);

                return View(lista);
            } catch (Exception)
            {
                TempData["Error"] = "Ocurrió un error al cargar el listado de inquilinos.";

                return View(new List<Inquilino>());
            }
        }

        // GET : /Inquilinos/Create
        public IActionResult Create()
        {
            return View();
        }

        // GET : /Inquilinos/Edit/ID
        public IActionResult Edit(int id)
        {
            var inquilino = _service.ObtenerPorId(id);
            if (inquilino == null)
            {
                return NotFound();
            }
            return View(inquilino);
        }

        // POST : /Inquilinos/Guardar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Guardar([FromBody] Inquilino inquilino)
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
                if (inquilino.Id == 0)
                {
                    var nuevoId = _service.Alta(inquilino);
                    return Ok(new { success = true, message = "Inquilino creado correctamente.", data = new { id = nuevoId }});
                }
                else
                {
                    _service.Modificacion(inquilino);
                    return Ok(new { success = true, message = "Inquilino actualizado correctamente."});
                }
            }
            catch (AppException ex)
            {
                return BadRequest(new { success = false, message = ex.Message});
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "Ocurrió un error inesperado al guardar el inquilino."});
            }
        }

        // POST: /Inquilinos/Eliminar/ID
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(int id)
        {
            try
            {
                _service.Baja(id);
                return Ok(new { success = true, message = "Inquilino dado de baja correctamente."});
            }
            catch (AppException ex)
            {
                return BadRequest(new { success = false, message = ex.Message});
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "Ocurrió un error inesperado al eliminar el inquilino."});
            }
        }
    }
}