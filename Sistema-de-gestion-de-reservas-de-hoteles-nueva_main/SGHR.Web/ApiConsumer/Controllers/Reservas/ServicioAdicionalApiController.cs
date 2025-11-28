using Microsoft.AspNetCore.Mvc;
using SGHR.Application.DTOs.Reservas.ServicioAdicional;
using SGHR.Domain.Base;
using SGHR.Web.Infrastructure.Services.Api.Interfaces;
using SGHR.Web.Helpers;

namespace SGHR.Web.ApiConsumer.Controllers.Reservas
{
    public class ServicioAdicionalApiController : Controller
    {
        private readonly ILogger<ServicioAdicionalApiController> _logger;
        private readonly IServicioAdicionalApiService _servicioAdicionalApiService;

        public ServicioAdicionalApiController(
            IServicioAdicionalApiService servicioAdicionalApiService,
            ILogger<ServicioAdicionalApiController> logger)
        {
            _servicioAdicionalApiService = servicioAdicionalApiService ?? throw new ArgumentNullException(nameof(servicioAdicionalApiService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IActionResult> Index()
        {
            var result = await _servicioAdicionalApiService.GetAllAsync();
            var servicios = result?.Data ?? new List<ServicioAdicionalDTO>();
            return View(servicios);
        }

        public async Task<IActionResult> _List()
        {
            var result = await _servicioAdicionalApiService.GetAllAsync();

            if (ErrorHelper.IsSuccess(result, out string? errorMessage))
            {
                TempData["Success"] = result.Message ?? SuccessMessages.Loaded;
                return PartialView("_List", result.Data ?? new List<ServicioAdicionalDTO>());
            }

            TempData["Error"] = errorMessage;
            return PartialView("_List", new List<ServicioAdicionalDTO>());
        }

        public async Task<IActionResult> Details(int id)
        {
            if (!ErrorHelper.IsValidId(id, out string? idError))
            {
                TempData["Error"] = idError;
                return RedirectToAction("Index");
            }

            var result = await _servicioAdicionalApiService.GetByIdAsync(id);

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
            var model = new CreateServicioAdicionalDTO();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateServicioAdicionalDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _servicioAdicionalApiService.CreateAsync(model);

            if (ErrorHelper.IsSuccess(result, out string? errorMessage))
            {
                TempData["Success"] = result.Message ?? SuccessMessages.GetCreatedMessage("Servicio Adicional");
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

            var result = await _servicioAdicionalApiService.GetByIdAsync(id);

            if (ErrorHelper.IsSuccess(result, out string? errorMessage) && result.Data != null)
            {
                var servicio = result.Data;
                var updateDto = new UpdateServicioAdicionalDTO
                {
                    Id = servicio.Id,
                    Nombre = servicio.Nombre,
                    Precio = servicio.Precio,
                    Descripcion = servicio.Descripcion,
                    Estado = servicio.Estado
                };

                TempData["Success"] = result.Message;
                return View(updateDto);
            }

            TempData["Error"] = errorMessage;
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateServicioAdicionalDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _servicioAdicionalApiService.UpdateAsync(model);

            if (ErrorHelper.IsSuccess(result, out string? errorMessage))
            {
                TempData["Success"] = result.Message ?? SuccessMessages.GetUpdatedMessage("Servicio Adicional");
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

            var result = await _servicioAdicionalApiService.GetByIdAsync(id);

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

            var result = await _servicioAdicionalApiService.DeleteAsync(id);

            if (ErrorHelper.IsSuccess(result, out string? errorMessage))
            {
                return Json(new { success = true, message = result.Message ?? SuccessMessages.GetDeletedMessage("Servicio Adicional"), data = result.Data });
            }

            return Json(new { success = false, message = errorMessage });
        }
    }
}
