using Microsoft.AspNetCore.Mvc;
using SGHR.Application.DTOs.Reservas.Tarifa;
using SGHR.Application.Interfaces.Reservas;

namespace SGHR.Web.Areas.Administrador.Controllers.Reservas
{
    [Area("Administrador")]
    public class TarifaController : Controller
    {
        private readonly ITarifaService _tarifaService;

        public TarifaController(ITarifaService tarifaService)
        {
            _tarifaService = tarifaService;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _tarifaService.GetAllAsync();

            if (!result.Success || result.Data == null)
                return View(new List<TarifaDTO>());

            return View(result.Data);
        }

        public IActionResult Create()
        {
            return View(new CreateTarifaDTO());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTarifaDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _tarifaService.CreateAsync(dto);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                return View(dto);
            }

            TempData["Success"] = "Tarifa creada correctamente";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var result = await _tarifaService.GetByIdAsync(id);

            if (!result.Success || result.Data == null)
                return NotFound();

            var dto = new UpdateTarifaDTO
            {
                Id = result.Data.Id,
                Tipo = result.Data.Tipo,
                Monto = result.Data.Monto,
                FechaInicio = result.Data.FechaInicio,
                FechaFin = result.Data.FechaFin,
                PrecioPorNoche = result.Data.PrecioPorNoche,
                Descuento = result.Data.Descuento,
                Descripcion = result.Data.Descripcion,
                IdHabitacion = result.Data.IdHabitacion,
                Estado = result.Data.Estado
            };

            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateTarifaDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _tarifaService.UpdateAsync(dto);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                return View(dto);
            }

            TempData["Success"] = "Tarifa actualizada correctamente";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var result = await _tarifaService.GetByIdAsync(id);

            if (!result.Success || result.Data == null)
                return NotFound();

            return View(result.Data);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var result = await _tarifaService.GetByIdAsync(id);

            if (!result.Success || result.Data == null)
                return NotFound();

            return View(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dto = new DeleteTarifaDTO { Id = id };
            await _tarifaService.RemoveAsync(dto);

            TempData["Success"] = "Tarifa eliminada correctamente";
            return RedirectToAction(nameof(Index));
        }
    }
}
