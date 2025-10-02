using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Ciber.MVC.Models;
using Ciber.core;
using Ciber.Dapper;

namespace Ciber.MVC.Controllers;

public class HomeController : Controller
{
    private readonly IDAO _dao;
    private readonly ILogger<HomeController> _logger;

    public HomeController(IDAO dao, ILogger<HomeController> logger)
    {
        _dao = dao;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var cuentas = await _dao.ObtenerTodasLasCuentasAsync();
            var maquinas = await _dao.ObtenerTodasLasMaquinasAsync();
            var alquileres = await _dao.ObtenerTodosLosAlquileresAsync();
            var historial = await _dao.ObtenerTodoElHistorialAsync();

            ViewBag.TotalCuentas = cuentas.Count();
            ViewBag.TotalMaquinas = maquinas.Count();
            ViewBag.MaquinasDisponibles = maquinas.Count(m => m.Estado);
            ViewBag.MaquinasOcupadas = maquinas.Count(m => !m.Estado);
            ViewBag.AlquileresActivos = alquileres.Count();
            ViewBag.TotalHistorial = historial.Count();

            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar el dashboard");
            TempData["ErrorMessage"] = "Error al cargar la información del dashboard.";
            return View();
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
