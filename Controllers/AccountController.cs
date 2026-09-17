using System.Security.Claims;
using InmobiliariaApp.Models;
using InmobiliariaApp.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InmobiliariaApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUsuarioService _service;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IUsuarioService service, ILogger<AccountController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel());
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel modelo, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                return View(modelo);
            }

            Usuario? usuario;

            try
            {
                usuario = _service.ValidarCredenciales(modelo.Email, modelo.Password);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al validar credenciales de login.");
                ModelState.AddModelError(string.Empty, "Ocurrió un error inesperado. Intente nuevamente.");
                ViewBag.ReturnUrl = returnUrl;
                return View(modelo);
            }

            if (usuario == null)
            {
                ModelState.AddModelError(string.Empty, "Email o contraseña incorrectas.");
                ViewBag.ReturnUrl = returnUrl;
                return View(modelo);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name,$"{usuario.Nombre} {usuario.Apellido}"),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Rol?.Nombre ?? string.Empty),
                new Claim("Avatar", usuario.Avatar ?? string.Empty),
                new Claim("Iniciales", ObtenerIniciales(usuario.Nombre, usuario.Apellido))
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,

                principal,

                new AuthenticationProperties { IsPersistent = modelo.RecordarMe }
            );

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        private static string ObtenerIniciales(string nombre, string apellido)
        {
            var inicialNombre = nombre.Length > 0 ? nombre[0].ToString() : "";
            var inicialApellido = apellido.Length > 0 ? apellido[0].ToString() : "";
            return (inicialNombre + inicialApellido).ToUpper();
        }
    }
}
