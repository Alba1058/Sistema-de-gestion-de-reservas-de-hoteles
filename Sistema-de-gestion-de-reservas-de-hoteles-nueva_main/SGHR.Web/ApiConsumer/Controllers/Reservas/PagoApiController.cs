using Microsoft.AspNetCore.Mvc;
using SGHR.Application.DTOs.Reservas.Pago;
using SGHR.Domain.Base;
using SGHR.Web.Infrastructure.Services.Api.Interfaces;
using SGHR.Web.Helpers;

namespace SGHR.Web.ApiConsumer.Controllers.Reservas
{
    public class PagoApiController : Controller
    {
        private readonly ILogger<PagoApiController> _logger;
        private readonly IPagoApiService _pagoApiService;

        public PagoApiController(
            IPagoApiService pagoApiService,
            ILogger<PagoApiController> logger)
        {
            _pagoApiService = pagoApiService ?? throw new ArgumentNullException(nameof(pagoApiService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IActionResult> Index()
        {
            var result = await _pagoApiService.GetAllAsync();
            var pagos = result?.Data ?? new List<PagoDTO>();
            return View(pagos);
        }

        public async Task<IActionResult> _List()
        {
            var result = await _pagoApiService.GetAllAsync();
            return PartialView("_List", result?.Data ?? new List<PagoDTO>());
        }

        public async Task<IActionResult> Details(int id)
        {
            if (!ErrorHelper.IsValidId(id, out string? idError))
            {
                TempData["Error"] = idError;
                return RedirectToAction("Index");
            }

            var result = await _pagoApiService.GetByIdAsync(id);

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
            return View(new CreatePagoDTO());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePagoDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);


            var result = await _pagoApiService.CreateAsync(model);

            if (ErrorHelper.IsSuccess(result, out string? errorMessage))
            {
                TempData["Success"] = result.Message ?? SuccessMessages.GetCreatedMessage("Pago");
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

            var result = await _pagoApiService.GetByIdAsync(id);

            if (ErrorHelper.IsSuccess(result, out string? errorMessage) && result.Data != null)
            {
                var pago = result.Data;
                var updateDto = new UpdatePagoDTO
                {
                    Id = pago.Id,
                    IdReserva = pago.IdReserva,
                    Monto = pago.Monto,
                    FechaPago = pago.FechaPago,
                    Metodo = pago.Metodo,
                    Confirmado = pago.Confirmado,
                    Estado = pago.Estado
                };

                return View(updateDto);
            }

            TempData["Error"] = errorMessage;
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdatePagoDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);


            var result = await _pagoApiService.UpdateAsync(model);

            if (ErrorHelper.IsSuccess(result, out string? errorMessage))
            {
                TempData["Success"] = result.Message ?? SuccessMessages.GetUpdatedMessage("Pago");
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

            var result = await _pagoApiService.GetByIdAsync(id);

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

            var result = await _pagoApiService.DeleteAsync(id);

            if (ErrorHelper.IsSuccess(result, out string? errorMessage))
            {
                return Json(new { success = true, message = result.Message ?? SuccessMessages.GetDeletedMessage("Pago"), data = result.Data });
            }

            return Json(new { success = false, message = errorMessage });
        }
    }
}
