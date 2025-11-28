using Microsoft.AspNetCore.Mvc;
using SGHR.Application.DTOs.Usuarios.Usuario;
using SGHR.Domain.Base;
using SGHR.Web.Infrastructure.Services.Api.Interfaces;
using SGHR.Web.Helpers;

namespace SGHR.Web.ApiConsumer.Controllers.Usuarios
{
    public class UsuarioApiController : Controller
    {
        private readonly ILogger<UsuarioApiController> _logger;
        private readonly IUsuarioApiService _usuarioApiService;

        public UsuarioApiController(
            IUsuarioApiService usuarioApiService,
            ILogger<UsuarioApiController> logger)
        {
            _usuarioApiService = usuarioApiService ?? throw new ArgumentNullException(nameof(usuarioApiService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IActionResult> Index()
        {
            var result = await _usuarioApiService.GetAllAsync();
            var usuarios = result?.Data ?? new List<UsuarioDTO>();
            return View(usuarios);
        }

        public async Task<IActionResult> _List()
        {
            var result = await _usuarioApiService.GetAllAsync();

            if (ErrorHelper.IsSuccess(result, out string? errorMessage))
            {
                TempData["Success"] = result.Message ?? SuccessMessages.Loaded;
                return PartialView("_List", result.Data ?? new List<UsuarioDTO>());
            }

            TempData["Error"] = errorMessage;
            return PartialView("_List", new List<UsuarioDTO>());
        }

        public async Task<IActionResult> Details(int id)
        {
            if (!ErrorHelper.IsValidId(id, out string? idError))
            {
                TempData["Error"] = idError;
                return RedirectToAction("Index");
            }

            var result = await _usuarioApiService.GetByIdAsync(id);

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
            var model = new UsuarioCreateDTO();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsuarioCreateDTO model)
        {

            if (!ModelState.IsValid)
            {
                var modelErrors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                
                
                TempData["Error"] = $"Errores de validación: {string.Join("; ", modelErrors)}";
                return View(model);
            }

            if (model == null)
            {
                _logger.LogError("El modelo es null");
                TempData["Error"] = "El modelo recibido es nulo";
                return View(new UsuarioCreateDTO());
            }

            var result = await _usuarioApiService.CreateAsync(model);

            if (ErrorHelper.IsSuccess(result, out string? errorMessage))
            {
                TempData["Success"] = result.Message ?? SuccessMessages.GetCreatedMessage("Usuario");
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

            var result = await _usuarioApiService.GetByIdAsync(id);

            if (ErrorHelper.IsSuccess(result, out string? errorMessage) && result.Data != null)
            {
                var usuario = result.Data;
                var updateDto = new UsuarioUpdateDTO
                {
                    Id = usuario.Id,
                    Nombre = usuario.Nombre,
                    Email = usuario.Email,
                    Activo = usuario.Activo,
                    RolUsuarioId = usuario.RolUsuarioId
                };

                TempData["Success"] = result.Message;
                return View(updateDto);
            }

            TempData["Error"] = errorMessage;
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UsuarioUpdateDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _usuarioApiService.UpdateAsync(model);

            if (ErrorHelper.IsSuccess(result, out string? errorMessage))
            {
                TempData["Success"] = result.Message ?? SuccessMessages.GetUpdatedMessage("Usuario");
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

            var result = await _usuarioApiService.GetByIdAsync(id);

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

            var result = await _usuarioApiService.DeleteAsync(id);

            if (ErrorHelper.IsSuccess(result, out string? errorMessage))
            {
                return Json(new { success = true, message = result.Message ?? SuccessMessages.GetDeletedMessage("Usuario"), data = result.Data });
            }

            return Json(new { success = false, message = errorMessage });
        }
    }
}
