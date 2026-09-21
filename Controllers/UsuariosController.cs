using InmobiliariaApp.Common.Exceptions;
using InmobiliariaApp.Models;
using InmobiliariaApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InmobiliariaApp.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UsuariosController : Controller
    {
        private readonly IUsuarioService _service;
        private readonly ILogger<UsuariosController> _logger;
        private const int TamPaginaDefault = 10;

        public UsuariosController(IUsuarioService service, ILogger<UsuariosController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // GET: /Usuarios
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar el listado de usuarios.");
                TempData["Error"] = "Ocurrió un error al cargar el listado de usuarios.";
                return View(new List<Usuario>());
            }
        }
        // GET: /Usuarios/Create
        public IActionResult Create()
        {
            return View(new Usuario());
        }
        // POST: /Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Usuario usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario.Password))
            {
                ModelState.AddModelError(nameof(usuario.Password), "La contraseña es obligatoria");
            }

            if (!ModelState.IsValid)
            {
                return View(usuario);
            }
            try
            {
                _service.Alta(usuario, usuario.Password!);
                TempData["Mensaje"] = "Usuario creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (AppException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear el usuario.");
                ModelState.AddModelError(string.Empty, "Ocurrió un error inesperado al crear el usuario.");
                return View(usuario);
            }
        }
        // GET: /Usuarios/Edit/ID
        public IActionResult Edit(int id)
        {
            var usuario = _service.ObtenerPorId(id);
            if (usuario == null) return NotFound();
            return View(usuario);
        }
        // POST: /Usuarios/ResetearPassword/ID
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Usuario usuario)
        {
            if (!ModelState.IsValid)
            {
                return View(usuario);
            }
            try
            {
                _service.Modificacion(usuario);
                TempData["Mensaje"] = "Usuario actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (AppException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al modificar el usuario.");
                ModelState.AddModelError(string.Empty, "Ocurrió un error inesperado al modificar el usuario.");
                return View(usuario);
            }
        }
        // POST: /Usuarios/ResetearPassword/ID
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetearPassword(int id, string nuevaPassword)
        {
            try
            {
                _service.ResetearPassword(id, nuevaPassword);
                TempData["Mesaje"] = "Contraseña reseteada correctamente.";
            }
            catch (AppException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al resetear la contraseña.");
                TempData["Error"] = "Ocurrió un error inesperado.";
            }
            return RedirectToAction(nameof(Edit), new { id });
        }
        // POST: /Usuarios/Eliminar/ID
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(int id)
        {
            try
            {
                _service.Baja(id);
                TempData["Mensaje"] = "Usuario dedo de baja correctamente.";
            }
            catch (AppException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar el usuario.");
                TempData["Error"] = "Ocurrió un error inesperado.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
