using Microsoft.AspNetCore.Mvc;
using Ciber.core;
using Ciber.Dapper;

namespace Ciber.MVC.Controllers
{
    public class AlquilerController : Controller
    {
        private readonly IDAO _dao;

        public AlquilerController(IDAO dao)
        {
            _dao = dao;
        }

        // GET: Alquiler
        public async Task<IActionResult> Index()
        {
            var alquileres = await _dao.ObtenerTodosLosAlquileresAsync();
            return View(alquileres);
        }

        // GET: Alquiler/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var alquiler = await _dao.ObtenerAlquilerPorIdAsync(id);
            if (alquiler == null)
            {
                return NotFound();
            }
            return View(alquiler);
        }

        // GET: Alquiler/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Cuentas = await _dao.ObtenerTodasLasCuentasAsync();
            ViewBag.Maquinas = await _dao.ObtenerTodasLasMaquinasAsync();
            return View();
        }

        // POST: Alquiler/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Alquiler alquiler, bool tipoAlquiler)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _dao.AgregarAlquilerAsync(alquiler, tipoAlquiler);
                    TempData["SuccessMessage"] = "Alquiler creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error al crear el alquiler: {ex.Message}";
                }
            }
            
            ViewBag.Cuentas = await _dao.ObtenerTodasLasCuentasAsync();
            ViewBag.Maquinas = await _dao.ObtenerTodasLasMaquinasAsync();
            return View(alquiler);
        }

        // GET: Alquiler/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var alquiler = await _dao.ObtenerAlquilerPorIdAsync(id);
            if (alquiler == null)
            {
                return NotFound();
            }
            return View(alquiler);
        }

        // POST: Alquiler/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _dao.EliminarAlquilerAsync(id);
                TempData["SuccessMessage"] = "Alquiler eliminado exitosamente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al eliminar el alquiler: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Alquiler/Finalizar/5
        public async Task<IActionResult> Finalizar(int id)
        {
            var alquiler = await _dao.ObtenerAlquilerPorIdAsync(id);
            if (alquiler == null)
            {
                return NotFound();
            }
            
            // Calcular el tiempo transcurrido y el total aproximado
            var tiempoTranscurrido = DateTime.Now - alquiler.FechaInicio;
            var horasRedondeadas = Math.Ceiling(tiempoTranscurrido.TotalHours);
            var totalEstimado = (decimal)horasRedondeadas * alquiler.PrecioPorHora;
            
            ViewBag.TiempoTranscurrido = tiempoTranscurrido;
            ViewBag.TotalEstimado = totalEstimado;
            
            return View(alquiler);
        }

        // POST: Alquiler/Finalizar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Finalizar(int id, decimal montoPagado)
        {
            try
            {
                await _dao.FinalizarAlquilerAsync(id, montoPagado);
                TempData["SuccessMessage"] = "Alquiler finalizado exitosamente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al finalizar el alquiler: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
