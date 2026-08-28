using InmobiliariaApp.Common.Exceptions;
using InmobiliariaApp.Models;
using InmobiliariaApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InmobiliariaApp.Controllers
{
    public class PropietariosController : Controller
    {
        private readonly IPropietarioService _service;
        private const int TamPaginaDefault = 10;

        public PropietariosController(IPropietarioService service)
        {
            _service = service;
        }

        //GET : /Propietarios

        public IActionResult Index(int paginaNro = 1)
        {
            try
            {
                var lista = _service.ObtenerLista(paginaNro, TamPaginaDefault);
                var cantidadTotal = _service.ObtenerCantidad();

                ViewBag.PaginaNro = paginaNro;
                ViewBag.TotalPaginas = (int)Math.Ceiling(cantidadTotal / (double)TamPaginaDefault);
                ViewBag.CantidadInactivos = _service.ObtenerCantidadInactivos();

                return View(lista);
            }
            catch (Exception)
            {
                TempData["Error"] = "Ocurrió un error al cargar el listado de propietarios.";

                return View(new List<Propietario>());
            }
        }
        //GET : /Propietarios/Create
        public IActionResult Create()
        {
            return View();
        }

        //GET : /Propietarios/Edit/ID
        public IActionResult Edit(int id)
        {
            var propietario = _service.ObtenerPorId(id);
            if (propietario == null)
            {
                return NotFound();
            }
            return View(propietario);
        }
        // GET : /Propietarios/Inactivos
        public IActionResult Inactivos()
        {
            try
            {
                var lista = _service.ObtenerListaInactivos();
                return View(lista);
            }
            catch (Exception)
            {
                TempData["Error"] = "Ocurrió un error al cargar el listado de propietarios inactivos.";

                return View(new List<Propietario>());

            }
        }
        // POST: /Propietarios/Guardar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Guardar([FromBody] Propietario propietario)
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
                if (propietario.Id == 0)
                {
                    var nuevoId = _service.Alta(propietario);
                    return Ok(new { success = true, message = "Propietario creado correctamente.", data = new { id = nuevoId } });
                }
                else
                {
                    _service.Modificacion(propietario);
                    return Ok(new { success = true, message = "Propietario actualizado correctamente." });
                }
            }
            catch (AppException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "Ocurrió un error inesperado al guardar el propietario." });
            }
        }

        // POST: /Propietarios/Eliminar/ID
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(int id)
        {
            try
            {
                _service.Baja(id);
                return Ok(new { success = true, message = "Propietario dado de baja correctamente." });
            }
            catch (AppException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "Ocurrió un error inesperado al eliminar el propietario." });
            }
        }

        // POST: /Propietarios/Reactivar/ID
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reactivar(int id)
        {
            try
            {
                _service.Reactivar(id);
                return Ok(new { success = true, message = "Propietario reactivado correctamente." });
            }
            catch (AppException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "Ocurrió un error inesperado al reactivar el propietario." });
            }
        }
    }

}
