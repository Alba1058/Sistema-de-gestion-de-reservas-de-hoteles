using Microsoft.AspNetCore.Mvc;
using SGHR.Application.DTOs.Reservas.Tarifa;
using SGHR.Domain.Base;
using SGHR.Web.Infrastructure.Services.Api.Interfaces;
using SGHR.Web.Helpers;

namespace SGHR.Web.ApiConsumer.Controllers.Reservas
{
    public class TarifaApiController : Controller
    {
        private readonly ILogger<TarifaApiController> _logger;
        private readonly ITarifaApiService _tarifaApiService;

        public TarifaApiController(
            ITarifaApiService tarifaApiService,
            ILogger<TarifaApiController> logger)
        {
            _tarifaApiService = tarifaApiService ?? throw new ArgumentNullException(nameof(tarifaApiService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IActionResult> Index()
        {
            var result = await _tarifaApiService.GetAllAsync();
            var tarifas = result?.Data ?? new List<TarifaDTO>();
            return View(tarifas);
        }

        public async Task<IActionResult> _List()
        {
            var result = await _tarifaApiService.GetAllAsync();
            return PartialView("_List", result?.Data ?? new List<TarifaDTO>());
        }

        public async Task<IActionResult> Details(int id)
        {
            if (!ErrorHelper.IsValidId(id, out string? idError))
            {
                TempData["Error"] = idError;
                return RedirectToAction("Index");
            }

            var result = await _tarifaApiService.GetByIdAsync(id);

            if (ErrorHelper.IsSuccess(result, out string? errorMessage) && result.Data != null)
            {
                return View(result.Data);
            }

            TempData["Error"] = errorMessage;
            return RedirectToAction("Index");
        }

        public IActionResult Create()
        {
            TempData.Remove("Success");
            TempData.Remove("Error");
            return View(new CreateTarifaDTO());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTarifaDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);


            var result = await _tarifaApiService.CreateAsync(model);

            if (ErrorHelper.IsSuccess(result, out string? errorMessage))
            {
                TempData["Success"] = result.Message ?? SuccessMessages.GetCreatedMessage("Tarifa");
                return RedirectToAction("Index");
            }

            TempData["Error"] = errorMessage;
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (!ErrorHelper.IsValidId(id, out string? idError))
            {
                TempData["Error"] = idError;
                return RedirectToAction("Index");
            }

            var result = await _tarifaApiService.GetByIdAsync(id);

            if (ErrorHelper.IsSuccess(result, out string? errorMessage) && result.Data != null)
            {
                var tarifa = result.Data;
                var updateDto = new UpdateTarifaDTO
                {
                    Id = tarifa.Id,
                    Tipo = tarifa.Tipo,
                    Monto = tarifa.Monto,
                    FechaInicio = tarifa.FechaInicio,
                    FechaFin = tarifa.FechaFin,
                    PrecioPorNoche = tarifa.PrecioPorNoche,
                    Descuento = tarifa.Descuento,
                    Descripcion = tarifa.Descripcion,
                    IdHabitacion = tarifa.IdHabitacion,
                    Estado = tarifa.Estado
                };

                return View(updateDto);
            }

            TempData["Error"] = errorMessage;
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateTarifaDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);


            var result = await _tarifaApiService.UpdateAsync(model);

            if (ErrorHelper.IsSuccess(result, out string? errorMessage))
            {
                TempData["Success"] = result.Message ?? SuccessMessages.GetUpdatedMessage("Tarifa");
                return RedirectToAction("Index");
            }

            TempData["Error"] = errorMessage;
            return View(model);
        }

        public async Task<IActionResult> _Delete(int id)
        {
            if (!ErrorHelper.IsValidId(id, out string? idError))
            {
                TempData["Error"] = idError;
                return RedirectToAction("Index");
            }

            var result = await _tarifaApiService.GetByIdAsync(id);

            if (ErrorHelper.IsSuccess(result, out string? errorMessage) && result.Data != null)
            {
                return PartialView("_Delete", result.Data);
            }

            TempData["Error"] = errorMessage;
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ActionName("_DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _DeleteConfirmed(int id)
        {
            if (!ErrorHelper.IsValidId(id, out string? idError))
            {
                return Json(new { success = false, message = idError });
            }

            var result = await _tarifaApiService.DeleteAsync(id);

            if (ErrorHelper.IsSuccess(result, out string? errorMessage))
            {
                return Json(new { success = true, message = result.Message ?? SuccessMessages.GetDeletedMessage("Tarifa"), data = result.Data });
            }

            return Json(new { success = false, message = errorMessage });
        }
    }
}
