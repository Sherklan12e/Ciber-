using Microsoft.AspNetCore.Mvc;
using Ciber.core;
using Ciber.Dapper;

namespace Ciber.MVC.Controllers
{
    public class MaquinaController : Controller
    {
        private readonly IDAO _dao;

        public MaquinaController(IDAO dao)
        {
            _dao = dao;
        }

        // GET: Maquina
        public async Task<IActionResult> Index()
        {
            var maquinas = await _dao.ObtenerTodasLasMaquinasAsync();
            return View(maquinas);
        }

        // GET: Maquina/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var maquina = await _dao.ObtenerMaquinaPorIdAsync(id);
            if (maquina == null)
            {
                return NotFound();
            }
            return View(maquina);
        }

        // GET: Maquina/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Maquina/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Maquina maquina)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _dao.AgregarMaquinaAsync(maquina);
                    TempData["SuccessMessage"] = "Máquina creada exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error al crear la máquina: {ex.Message}";
                }
            }
            return View(maquina);
        }

        // GET: Maquina/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var maquina = await _dao.ObtenerMaquinaPorIdAsync(id);
            if (maquina == null)
            {
                return NotFound();
            }
            return View(maquina);
        }

        // POST: Maquina/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Maquina maquina)
        {
            if (id != maquina.Nmaquina)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _dao.ActualizarMaquinaAsync(maquina);
                    TempData["SuccessMessage"] = "Máquina actualizada exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error al actualizar la máquina: {ex.Message}";
                }
            }
            return View(maquina);
        }

        // GET: Maquina/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var maquina = await _dao.ObtenerMaquinaPorIdAsync(id);
            if (maquina == null)
            {
                return NotFound();
            }
            return View(maquina);
        }

        // POST: Maquina/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _dao.EliminarMaquinaAsync(id);
                TempData["SuccessMessage"] = "Máquina eliminada exitosamente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al eliminar la máquina: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Maquina/Disponibles
        public async Task<IActionResult> Disponibles()
        {
            var maquinasDisponibles = await _dao.ObtenerMaquinaDisponiblesAsync();
            return View("Index", maquinasDisponibles);
        }

        // GET: Maquina/Ocupadas
        public async Task<IActionResult> Ocupadas()
        {
            var maquinasOcupadas = await _dao.ObtenerMaquinaNoDisponiblesesAsync();
            return View("Index", maquinasOcupadas);
        }
    }
}