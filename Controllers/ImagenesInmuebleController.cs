using InmobiliariaApp.Common.Exceptions;
using InmobiliariaApp.Models;
using InmobiliariaApp.Repositories.Interfaces;
using InmobiliariaApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InmobiliariaApp.Controllers
{
    public class ImagenesInmuebleController : Controller
    {
        private readonly IImagenesInmuebleService _service;
        private const int TamPaginaDefault = 10;

        public ImagenesInmuebleController(IImagenesInmuebleService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Alta(int id, List<IFormFile> ImagenesInmueble, [FromServices] IWebHostEnvironment environment)
        {
            if (ImagenesInmueble == null || ImagenesInmueble.Count == 0)
            {
                return BadRequest("No se recibieron archivos.");
            }
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
            path = Path.Combine(path, id.ToString());
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            foreach(var file in ImagenesInmueble)
            {
                if (file.Length > 0)
                {
                    var extension = Path.GetExtension(file.FileName);
                    var nombreArchivo = $"{Guid.NewGuid()}{extension}";
                    var rutaArchivo = Path.Combine(path, nombreArchivo);
                    using (var stream = new FileStream(rutaArchivo, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    ImagenesInmueble imagenesInmueble = new ImagenesInmueble
                    {
                        InmuebleId = id,
                        ImgURL = $"/Uploads/Inmuebles/{id}/{nombreArchivo}",
                    };
                    _service.Alta(imagenesInmueble);
                }
            }
            var listaActualizada = _service.BuscarPorInmueble(id);
            return Json(listaActualizada);
        }

        // Eliminar
        [HttpPost]
        //[Authorize(Policy = "Administrador")]
        public ActionResult Eliminar(int id, [FromServices] IWebHostEnvironment environment)
        {
            try
            {
                var entidad = _service.ObtenerPorId(id);
                if (entidad == null)
                {
                    return NotFound("La imagen no existe.");
                }
                _service.Baja(id);
                if(!string.IsNullOrEmpty(entidad.ImgURL))
                {
                    string rutaRelativa = entidad.ImgURL.TrimStart('/', '\\');
                    string rutaArchivo = Path.Combine(environment.WebRootPath, rutaRelativa);
                    if (System.IO.File.Exists(rutaArchivo))
                    {
                        System.IO.File.Delete(rutaArchivo);
                    }
                }
                var listaActualizada = _service.BuscarPorInmueble(entidad.InmuebleId);
                return Json(listaActualizada);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}