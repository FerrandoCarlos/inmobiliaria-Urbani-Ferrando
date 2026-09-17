using System.Security.Cryptography;
using InmobiliariaApp.Common.Exceptions;
using InmobiliariaApp.Models;
using InmobiliariaApp.Repositories.Interfaces;
using InmobiliariaApp.Services.Implementations;
using InmobiliariaApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InmobiliariaApp.Controllers
{
    public class InmueblesController : Controller
    {
        private readonly IInmuebleService _service;
        private readonly ITipoInmuebleService _tipoService;
        private readonly IPropietarioService _propietarioService;
        private readonly IInmuebleFiltroService _inmuebleFiltroService;
        private const int TamPaginaDefault = 10;

        public InmueblesController(IInmuebleService service, ITipoInmuebleService tipoService, IPropietarioService propietarioService, IInmuebleFiltroService inmuebleFiltroService)
        {
            _service = service;
            _tipoService = tipoService;
            _propietarioService = propietarioService;
            _inmuebleFiltroService = inmuebleFiltroService;
        }

        //GET : /Inmuebles

        public IActionResult Index(InmuebleFiltro filtro, int paginaNro = 1)
        {
            ViewBag.Propietarios = _propietarioService.ObtenerLista();
            ViewBag.TiposInmueble = _tipoService.ObtenerLista();
            ViewBag.FiltroActual = filtro ?? new InmuebleFiltro();
            try
            {
                var listaFiltrada = _inmuebleFiltroService.ObtenerPorFiltro(filtro ?? new InmuebleFiltro());
                int cantidadTotal = listaFiltrada.Count;
                int paginasTotal = (int)Math.Ceiling(cantidadTotal / (double)TamPaginaDefault);
                if (paginaNro < 1) paginaNro = 1;
                if (paginasTotal > 0 && paginaNro > paginasTotal) paginaNro = paginasTotal;
                var listaPaginada = listaFiltrada.Skip((paginaNro - 1) * TamPaginaDefault).Take(TamPaginaDefault).ToList();
                ViewBag.PaginaNro = paginaNro;
                ViewBag.TotalPaginas = paginasTotal;
                return View(listaPaginada);
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
            ViewBag.TipoInmueble = _tipoService.ObtenerLista(1, 100) ?? new List<TipoInmueble>();
            return View();
        }

        // GET: /Inmuebles/ImagenesInmueble/ID
        public ActionResult ImagenesInmueble(int id, [FromServices] IImagenesInmuebleRepository repoImagen)
        {
            var entidad = _service.ObtenerPorId(id);
            if (entidad == null)
                return NotFound();
            entidad.Imagenes = repoImagen.BuscarPorInmueble(id);
            return View(entidad);
        }

        // GET : /Inmuebles/Edit/ID

        public IActionResult Edit(int id)
        {
            var inmueble = _service.ObtenerPorId(id);
            if (inmueble == null)
            {
                return NotFound();
            }
            ViewBag.TipoInmueble = _tipoService.ObtenerLista(1, 100) ?? new List<TipoInmueble>();
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

        // POST : /Inmuebles/ImgPortadaURL
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ImgPortadaURL(ImagenesInmueble entidad, [FromServices] IWebHostEnvironment environment)
        {
            try
            {
                var inmueble = _service.ObtenerPorId(entidad.InmuebleId);
                if (inmueble == null)
                {
                    TempData["Error"] = "El inmueble no existe.";
                    return RedirectToAction(nameof(Index));
                }
                if (!string.IsNullOrEmpty(inmueble.ImgPortadaURL))
                {
                    string rutaRelativaAnterior = inmueble.ImgPortadaURL.TrimStart('/', '\\');
                    string rutaEliminar = Path.Combine(environment.WebRootPath, rutaRelativaAnterior);
                    if (System.IO.File.Exists(rutaEliminar))
                    {
                        System.IO.File.Delete(rutaEliminar);
                    }
                }

                if (entidad.Archivo != null && entidad.Archivo.Length > 0)
                {
                    string wwwPath = environment.WebRootPath;
                    string path = Path.Combine(wwwPath, "Uploads");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    path = Path.Combine(path, "Inmuebles");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    string extension = Path.GetExtension(entidad.Archivo.FileName);
                    string fileName = $"Portada_{entidad.InmuebleId}_{Guid.NewGuid()}{extension}";
                    string rutaFisicaCompleta = Path.Combine(path, fileName);
                    using (var stream = new FileStream(rutaFisicaCompleta, FileMode.Create))
                    {
                        entidad.Archivo.CopyTo(stream);
                    }
                    entidad.ImgURL = $"/Uploads/Inmuebles/{fileName}";
                }
                else
                {
                    entidad.ImgURL = string.Empty;
                }
                _service.ModificarPortada(entidad.InmuebleId, entidad.ImgURL);
                TempData["Mensaje"] = "Portada actualizada correctamente";
                return RedirectToAction(nameof(ImagenesInmueble), new { id = entidad.InmuebleId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(ImagenesInmueble), new { id = entidad.InmuebleId });
            }
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

                return BadRequest(new { success = false, message = string.Join(" ", errores) });
            }

            try
            {
                var propietarioExiste = _propietarioService.ObtenerPorId(inmueble.PropietarioId);
                if (propietarioExiste == null)
                {
                    return BadRequest(new { success = false, message = $"El propietario con ID {inmueble.PropietarioId} no existe." });
                }
                var tipoExiste = _tipoService.ObtenerPorId(inmueble.TipoInmuebleId);
                if (tipoExiste == null)
                {
                    return BadRequest(new { success = false, message = $"El TipoInmueble con ID {inmueble.TipoInmuebleId} no existe." });
                }
                if (inmueble.Id == 0)
                {
                    var nuevoId = _service.Alta(inmueble);
                    return Ok(new { success = true, message = "Inmueble creado correctamente.", data = new { id = nuevoId } });
                }
                else
                {
                    _service.Modificacion(inmueble);
                    return Ok(new { success = true, message = "Inmueble actualizado correctamente. " });
                }
            }
            catch (AppException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
                //return StatusCode(500, new { success = false, message = "Ocurrió un error inesperado al guardar el inmueble."});
            }
        }

        // POST : /Inmuebles/Eliminar/ID
        [Authorize(Roles = "Administrador")]
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
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "Ocurrió un error inesperado al eliminar el inmueble." });
            }
        }
    }
}
