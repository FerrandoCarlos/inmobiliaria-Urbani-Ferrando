using InmobiliariaApp.Common.Exceptions;
using InmobiliariaApp.Models;
using InmobiliariaApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InmobiliariaApp.Controllers
{
    public class InmueblesController : Controller
    {
        private readonly IInmuebleService _service;
        private const int TamPaginaDefault = 10;

        public InmueblesController(IInmuebleService service)
        {
            _service = service;
        }

        //GET : /Inmuebles

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
                TempData["Error"] = "Ocurrió un error al cargar el listado de inmuebles.";

                return View(new List<Inmueble>());
            }
        }

        // GET : /Inmuebles/Create

        public IActionResult Create()
        {
            return View();
        }

        // GET : /Inmuebles/Edit/ID

        public IActionResult Edit(int id)
        {
            var inmueble = _service.ObtenerPorId(id);
            if (inmueble == null)
            {
                return NotFound();
            }
            return View(inmueble);
        }

        // GET : /Inmuebles/EditPortada/ID

        public IActionResult EditPortada(int id)
        {
            var inmueble = _service.ObtenerPorId(id);
            if (inmueble == null)
            {
                return NotFound();
            }
            return View(inmueble);
        }

        // GET : /Inmuebles/EditEstado/ID

        public IActionResult EditEstado(int id)
        {
            var inmueble = _service.ObtenerPorId(id);
            if (inmueble == null)
            {
                return NotFound();
            }

            return View(inmueble);
        }

        //POST: /Inmuebles/Guardar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Guardar([FromBody] Inmueble inmueble)
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
                if (inmueble.Id == 0)
                {
                    var nuevoId = _service.Alta(inmueble);
                    return Ok(new { success = true, message = "Inmueble creado correctamente.", data = new { id = nuevoId} });
                }
                else
                {
                    _service.Modificacion(inmueble);
                    return Ok(new { success = true, message = "Inmueble actualizado correctamente. "});
                }
            } catch (AppException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            } catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "Ocurrió un error inesperado al guardar el inmueble."});
            }
        }

        // POST : /Inmuebles/Eliminar/ID
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(int id)
        {
            try
            {
                _service.Baja(id);
                return Ok(new { success = true, message = "Inmueble dado de baja correctamente." });
            }
            catch (AppException ex)
            {
                return BadRequest(new { success = false, message = ex.Message});
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "Ocurrió un error inesperado al eliminar el inmueble."});
            }
        }
    }
}