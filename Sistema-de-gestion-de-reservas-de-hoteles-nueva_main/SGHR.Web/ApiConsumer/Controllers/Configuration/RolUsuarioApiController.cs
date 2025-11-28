using Microsoft.AspNetCore.Mvc;
using SGHR.Application.DTOs.Configuration.RolUsuario;
using SGHR.Domain.Base;
using SGHR.Web.Infrastructure.Services.Api.Interfaces;
using SGHR.Web.Helpers;

namespace SGHR.Web.ApiConsumer.Controllers.Configuration
{
    public class RolUsuarioApiController : Controller
    {
        private readonly ILogger<RolUsuarioApiController> _logger;
        private readonly IRolUsuarioApiService _rolUsuarioApiService;

        public RolUsuarioApiController(
            IRolUsuarioApiService rolUsuarioApiService,
            ILogger<RolUsuarioApiController> logger)
        {
            _rolUsuarioApiService = rolUsuarioApiService ?? throw new ArgumentNullException(nameof(rolUsuarioApiService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IActionResult> Index()
        {
            var result = await _rolUsuarioApiService.GetAllAsync();
            var roles = result?.Data ?? new List<RolUsuarioDTO>();
            return View(roles);
        }

        public async Task<IActionResult> _List()
        {
            var result = await _rolUsuarioApiService.GetAllAsync();

            if (ErrorHelper.IsSuccess(result, out string? errorMessage))
            {
                TempData["Success"] = result.Message ?? SuccessMessages.Loaded;
                return PartialView("_List", result.Data ?? new List<RolUsuarioDTO>());
            }

            TempData["Error"] = errorMessage;
            return PartialView("_List", new List<RolUsuarioDTO>());
        }

        public async Task<IActionResult> Details(int id)
        {
            if (!ErrorHelper.IsValidId(id, out string? idError))
            {
                TempData["Error"] = idError;
                return RedirectToAction("Index");
            }

            var result = await _rolUsuarioApiService.GetByIdAsync(id);

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
            var model = new CreateRolUsuarioDTO();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateRolUsuarioDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _rolUsuarioApiService.CreateAsync(model);

            if (ErrorHelper.IsSuccess(result, out string? errorMessage))
            {
                TempData["Success"] = result.Message ?? SuccessMessages.GetCreatedMessage("Rol de Usuario");
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

            var result = await _rolUsuarioApiService.GetByIdAsync(id);

            if (ErrorHelper.IsSuccess(result, out string? errorMessage) && result.Data != null)
            {
                var rol = result.Data;
                var updateDto = new UpdateRolUsuarioDTO
                {
                    Id = rol.Id,
                    Nombre = rol.Nombre,
                    Descripcion = rol.Descripcion,
                    Estado = rol.Estado
                };

                TempData["Success"] = result.Message;
                return View(updateDto);
            }

            TempData["Error"] = errorMessage;
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateRolUsuarioDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _rolUsuarioApiService.UpdateAsync(model);

            if (ErrorHelper.IsSuccess(result, out string? errorMessage))
            {
                TempData["Success"] = result.Message ?? SuccessMessages.GetUpdatedMessage("Rol de Usuario");
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

            var result = await _rolUsuarioApiService.GetByIdAsync(id);

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

            var result = await _rolUsuarioApiService.DeleteAsync(id);

            if (ErrorHelper.IsSuccess(result, out string? errorMessage))
            {
                return Json(new { success = true, message = result.Message ?? SuccessMessages.GetDeletedMessage("Rol de Usuario"), data = result.Data });
            }

            return Json(new { success = false, message = errorMessage });
        }
    }
}
