using System.Diagnostics;
using InmobiliariaApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using InmobiliariaApp.Models;

namespace InmobiliariaApp.Controllers;

public class HomeController : Controller
{
    private readonly IPropietarioService _propietarioService;
    private readonly IInquilinoService _inquilinoService;

    public HomeController(IPropietarioService propietarioService, IInquilinoService inquilinoService)
    {
        _propietarioService = propietarioService;
        _inquilinoService = inquilinoService;
    }
    public IActionResult Index()
    {
        ViewBag.CantidadPropietarios = _propietarioService.ObtenerCantidad();
        ViewBag.CantidadInquilinos = _inquilinoService.ObtenerCantidad();
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
