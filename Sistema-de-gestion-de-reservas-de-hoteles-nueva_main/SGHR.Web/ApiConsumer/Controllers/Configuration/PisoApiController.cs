using Microsoft.AspNetCore.Mvc;
using SGHR.Application.DTOs.Configuration.Piso;
using SGHR.Domain.Base;
using SGHR.Web.Infrastructure.Services.Api.Interfaces;
using SGHR.Web.Helpers;

namespace SGHR.Web.ApiConsumer.Controllers.Configuration
{
    public class PisoApiController : Controller
    {
        private readonly ILogger<PisoApiController> _logger;
        private readonly IPisoApiService _pisoApiService;

        public PisoApiController(
            IPisoApiService pisoApiService,
            ILogger<PisoApiController> logger)
        {
            _pisoApiService = pisoApiService ?? throw new ArgumentNullException(nameof(pisoApiService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IActionResult> Index()
        {
            var result = await _pisoApiService.GetAllAsync();
            var pisos = result?.Data ?? new List<PisoDTO>();
            return View(pisos);
        }

        public async Task<IActionResult> _List()
        {
            var result = await _pisoApiService.GetAllAsync();

            if (ErrorHelper.IsSuccess(result, out string? errorMessage))
            {
                TempData["Success"] = result.Message ?? SuccessMessages.Loaded;
                return PartialView("_List", result.Data ?? new List<PisoDTO>());
            }

            TempData["Error"] = errorMessage;
            return PartialView("_List", new List<PisoDTO>());
        }

        public async Task<IActionResult> Details(int id)
        {
            if (!ErrorHelper.IsValidId(id, out string? idError))
            {
                TempData["Error"] = idError;
                return RedirectToAction("Index");
            }

            var result = await _pisoApiService.GetByIdAsync(id);

            if (ErrorHelper.IsSuccess(result, out string? errorMessage) && result.Data != null)
            {
                TempData["Success"] = result.Message;
                return View(result.Data);
            }

            TempData["Error"] = errorMessage;
            return RedirectToAction("Index");
        }

        public IActionResult Create()
        {
            TempData.Remove("Success");
            TempData.Remove("Error");
            var model = new CreatePisoDTO();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePisoDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _pisoApiService.CreateAsync(model);

            if (ErrorHelper.IsSuccess(result, out string? errorMessage))
            {
                TempData["Success"] = result.Message ?? SuccessMessages.GetCreatedMessage("Piso");
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

            var result = await _pisoApiService.GetByIdAsync(id);

            if (ErrorHelper.IsSuccess(result, out string? errorMessage) && result.Data != null)
            {
                var piso = result.Data;
                var updateDto = new UpdatePisoDTO
                {
                    Id = piso.Id,
                    Numero = piso.Numero,
                    Descripcion = piso.Descripcion,
                    Estado = piso.Estado
                };

                TempData["Success"] = result.Message;
                return View(updateDto);
            }

            TempData["Error"] = errorMessage;
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdatePisoDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _pisoApiService.UpdateAsync(model);

            if (ErrorHelper.IsSuccess(result, out string? errorMessage))
            {
                TempData["Success"] = result.Message ?? SuccessMessages.GetUpdatedMessage("Piso");
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

            var result = await _pisoApiService.GetByIdAsync(id);

            if (ErrorHelper.IsSuccess(result, out string? errorMessage) && result.Data != null)
            {
                TempData["Success"] = result.Message;
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

            var result = await _pisoApiService.DeleteAsync(id);

            if (ErrorHelper.IsSuccess(result, out string? errorMessage))
            {
                return Json(new { success = true, message = result.Message ?? SuccessMessages.GetDeletedMessage("Piso"), data = result.Data });
            }

            return Json(new { success = false, message = errorMessage });
        }
    }
}
