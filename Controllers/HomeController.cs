using System.Diagnostics;
using InmobiliariaApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using InmobiliariaApp.Models;

namespace InmobiliariaApp.Controllers;

public class HomeController : Controller
{
    private readonly IPropietarioService _propietarioService;
    private readonly IInquilinoService _inquilinoService;

    private readonly IInmuebleService _inmuebleService;

    public HomeController(IPropietarioService propietarioService, IInquilinoService inquilinoService, IInmuebleService inmuebleService)
    {
        _propietarioService = propietarioService;
        _inquilinoService = inquilinoService;
        _inmuebleService = inmuebleService;
    }
    public IActionResult Index()
    {
        ViewBag.CantidadPropietarios = _propietarioService.ObtenerCantidad();
        ViewBag.CantidadInquilinos = _inquilinoService.ObtenerCantidad();
        ViewBag.CantidadInmuebles = _inmuebleService.ObtenerCantidad();
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
