using Microsoft.AspNetCore.Mvc;
using SGHR.Application.DTOs.Reservas.Habitacion;
using SGHR.Application.Interfaces.Reservas;
using SGHR.Application.Interfaces.Configuration;

namespace SGHR.Web.Areas.Administrador.Controllers.Reservas
{
    [Area("Administrador")]
    public class HabitacionController : Controller
    {
        private readonly IHabitacionService _habitacionService;
        private readonly ICategoriaService _categoriaService;
        private readonly IPisoService _pisoService;

        public HabitacionController(
            IHabitacionService habitacionService,
            ICategoriaService categoriaService,
            IPisoService pisoService)
        {
            _habitacionService = habitacionService;
            _categoriaService = categoriaService;
            _pisoService = pisoService;
        }

        // GET: Habitacion
        public async Task<IActionResult> Index()
        {
            var result = await _habitacionService.GetAllAsync();

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View();
            }

            return View(result.Data);
        }

        // GET: Habitacion/Details
        public async Task<IActionResult> Details(int id)
        {
            var result = await _habitacionService.GetByIdAsync(id);

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View();
            }

            return View(result.Data);
        }

        // GET: Habitacion/Create
        public async Task<IActionResult> Create()
        {
            await LoadDropdowns();
            return View();
        }

        // POST: Habitacion/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateHabitacionDTO dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return View(dto);
            }

            var result = await _habitacionService.CreateAsync(dto);

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                await LoadDropdowns();
                return View(dto);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Habitacion/Edit
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _habitacionService.GetByIdAsync(id);

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View();
            }

            var h = result.Data;

            var dto = new UpdateHabitacionDTO
            {
                Id = h.Id,
                Numero = h.Numero,
                IdCategoria = h.IdCategoria,
                IdPiso = h.IdPiso,
                EstadoHabitacion = h.EstadoHabitacion,
                PrecioBase = h.PrecioBase,
                Descripcion = h.Descripcion,
                Estado = h.Estado
            };

            await LoadDropdowns();
            return View(dto);
        }

        // POST: Habitacion/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateHabitacionDTO dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return View(dto);
            }

            var result = await _habitacionService.UpdateAsync(dto);

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                await LoadDropdowns();
                return View(dto);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Habitacion/Delete
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _habitacionService.GetByIdAsync(id);

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View();
            }

            return View(result.Data);
        }

        // POST: Habitacion/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dto = new DeleteHabitacionDTO { Id = id };

            var result = await _habitacionService.RemoveAsync(dto);

            if (!result.Success)
            {
                ViewBag.ErrorMessage = result.Message;
                return View("Delete");
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadDropdowns()
        {
            var categorias = await _categoriaService.GetAllAsync();
            var pisos = await _pisoService.GetAllAsync();

            ViewBag.Categorias = categorias.Data;
            ViewBag.Pisos = pisos.Data;
        }
    }
}
