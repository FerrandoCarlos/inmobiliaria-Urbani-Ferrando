using System.Security.Claims;
using InmobiliariaApp.Common.Exceptions;
using InmobiliariaApp.Models;
using InmobiliariaApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InmobiliariaApp.Controllers
{
    [Authorize]
    public class PerfilController : Controller
    {
        private readonly IUsuarioService _service;
        private readonly ILogger<PerfilController> _logger;

        private static readonly string[] ExtensionesPermitidas = { ".jpg", ".jpeg", ".png", ".webp" };

        public PerfilController(IUsuarioService service, ILogger<PerfilController> logger)
        {
            _service = service;
            _logger = logger;
        }

        private int UsuarioActualId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // GET: /Perfil
        public IActionResult Index()
        {
            var usuario = _service.ObtenerPorId(UsuarioActualId);
            if (usuario == null) return NotFound();

            var modelo = new PerfilViewModel
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Email = usuario.Email,
                AvatarActual = usuario.Avatar
            };
            return View(modelo);
        }
        // POST: /Perfil/ActualizarDatos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ActualizarDatos(string Nombre, string Apellido, IFormFile? AvatarFile, [FromServices] IWebHostEnvironment environment)
        {
            if (string.IsNullOrWhiteSpace(Nombre) || string.IsNullOrWhiteSpace(Apellido))
            {
                TempData["Error"] = "Nombre y apellido son obligatorios.";
                return RedirectToAction(nameof(Index));
            }
            try
            {
                string? avatarUrl = null;

                if (AvatarFile != null && AvatarFile.Length > 0)
                {
                    string extension = Path.GetExtension(AvatarFile.FileName).ToLowerInvariant();
                    if (!ExtensionesPermitidas.Contains(extension))
                    {
                        TempData["Error"] = "El avatar debe ser una imagen (jpg, png o webp)";
                        return RedirectToAction(nameof(Index));
                    }

                    var usuarioActual = _service.ObtenerPorId(UsuarioActualId);
                    if (usuarioActual != null && !string.IsNullOrEmpty(usuarioActual.Avatar))
                    {
                        string rutaAnterior = Path.Combine(environment.WebRootPath, "Uploads", "Avatares");
                        if (System.IO.File.Exists(rutaAnterior))
                        {
                            System.IO.File.Delete(rutaAnterior);
                        }
                    }

                    string carpeta = Path.Combine(environment.WebRootPath, "Uploads", "Avatares");
                    Directory.CreateDirectory(carpeta);

                    string nombreArchivo = $"Avatar_{UsuarioActualId}_{Guid.NewGuid()}{extension}";
                    string rutaCompleta = Path.Combine(carpeta, nombreArchivo);

                    using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                    {
                        AvatarFile.CopyTo(stream);
                    }
                    avatarUrl = $"/Uploads/Avatares/{nombreArchivo}";
                }

                _service.ActualizarPerfil(UsuarioActualId, Nombre, Apellido, avatarUrl);

                TempData["Mensaje"] = "Perfil actualizado. Los cambios se verán en la barra de navegación al volver a iniciar sesión";
                return RedirectToAction(nameof(Index));
            }
            catch (AppException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar el perfil.");
                TempData["Error"] = "Ocurrió un error inesperado al actualizar el perfil.";
                return RedirectToAction(nameof(Index));
            }
        }
        // POST: /Perfil/CambiarPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CambiarPassword(string passwordActual, string passwordNueva, string passwordConfirmar)
        {
            if (string.IsNullOrWhiteSpace(passwordActual) || string.IsNullOrWhiteSpace(passwordNueva))
            {
                TempData["Error"] = "Completa la contraseña actual y la nueva.";
                return RedirectToAction(nameof(Index));
            }
            if (passwordNueva != passwordConfirmar)
            {
                TempData["Error"] = "La contraseñas nuevas no coinciden.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _service.CambiarPasswordPropio(UsuarioActualId, passwordActual, passwordNueva);
                TempData["Mensaje"] = "Contraseña actualizada correctamente";
            }
            catch (AppException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al cambiar la contraseña.");
                TempData["Error"] = "Ocurrió un error inesperado.";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: /Perfil/EliminarAvatar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarAvatar([FromServices] IWebHostEnvironment environment)
        {
            try
            {
                var usuario = _service.ObtenerPorId(UsuarioActualId);
                if (usuario != null && !string.IsNullOrEmpty(usuario.Avatar))
                {
                    string ruta = Path.Combine(environment.WebRootPath, usuario.Avatar.TrimStart('/', '\\'));
                    if (System.IO.File.Exists(ruta))
                    {
                        System.IO.File.Delete(ruta);
                    }
                }

                _service.EliminarAvatar(UsuarioActualId);
                TempData["Mensaje"] = "Avatar eliminado.";
            }
            catch (AppException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar el avatar.");
                TempData["Error"] = "Ocurrió un error inesperado.";
            }
            return RedirectToAction(nameof(Index));
        }
    }

}
