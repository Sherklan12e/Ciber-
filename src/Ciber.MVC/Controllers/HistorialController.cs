using Microsoft.AspNetCore.Mvc;
using Ciber.core;
using Ciber.Dapper;

namespace Ciber.MVC.Controllers
{
    public class HistorialController : Controller
    {
        private readonly IDAO _dao;

        public HistorialController(IDAO dao)
        {
            _dao = dao;
        }

        // GET: Historial
        public async Task<IActionResult> Index()
        {
            var historial = await _dao.ObtenerTodoElHistorialAsync();
            return View(historial);
        }

        // GET: Historial/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var historial = await _dao.ObtenerHistorialPorIdAsync(id);
            if (historial == null)
            {
                return NotFound();
            }
            return View(historial);
        }

        // GET: Historial/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Cuentas = await _dao.ObtenerTodasLasCuentasAsync();
            ViewBag.Maquinas = await _dao.ObtenerTodasLasMaquinasAsync();
            return View();
        }

        // POST: Historial/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HistorialdeAlquiler historial)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _dao.AgregarHistorialAsync(historial);
                    TempData["SuccessMessage"] = "Registro de historial creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error al crear el registro de historial: {ex.Message}";
                }
            }
            
            ViewBag.Cuentas = await _dao.ObtenerTodasLasCuentasAsync();
            ViewBag.Maquinas = await _dao.ObtenerTodasLasMaquinasAsync();
            return View(historial);
        }
    }
}
