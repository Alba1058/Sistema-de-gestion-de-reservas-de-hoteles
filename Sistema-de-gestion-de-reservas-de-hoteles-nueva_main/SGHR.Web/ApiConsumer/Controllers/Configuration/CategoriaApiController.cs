using Microsoft.AspNetCore.Mvc;
using SGHR.Application.DTOs.Configuration.Categoria;
using SGHR.Domain.Base;
using SGHR.Web.Infrastructure.Services.Api.Interfaces;
using SGHR.Web.Helpers;

namespace SGHR.Web.ApiConsumer.Controllers.Configuration
{
    public class CategoriaApiController : Controller
    {
        private readonly ILogger<CategoriaApiController> _logger;
        private readonly ICategoriaApiService _categoriaApiService;

        public CategoriaApiController(
            ICategoriaApiService categoriaApiService,
            ILogger<CategoriaApiController> logger)
        {
            _categoriaApiService = categoriaApiService ?? throw new ArgumentNullException(nameof(categoriaApiService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IActionResult> Index()
        {
            var result = await _categoriaApiService.GetAllAsync();
            var categorias = result?.Data ?? new List<CategoriaDTO>();
            return View(categorias);
        }

        public async Task<IActionResult> _List()
        {
            var result = await _categoriaApiService.GetAllAsync();

            if (ErrorHelper.IsSuccess(result, out string? errorMessage))
            {
                TempData["Success"] = result.Message ?? SuccessMessages.Loaded;
                return PartialView("_List", result.Data ?? new List<CategoriaDTO>());
            }

            TempData["Error"] = errorMessage;
            return PartialView("_List", new List<CategoriaDTO>());
        }

        public async Task<IActionResult> Details(int id)
        {
            if (!ErrorHelper.IsValidId(id, out string? idError))
            {
                TempData["Error"] = idError;
                return RedirectToAction("Index");
            }

            var result = await _categoriaApiService.GetByIdAsync(id);

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
            var model = new CreateCategoriaDTO();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCategoriaDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _categoriaApiService.CreateAsync(model);

            if (ErrorHelper.IsSuccess(result, out string? errorMessage))
            {
                TempData["Success"] = result.Message ?? SuccessMessages.GetCreatedMessage("Categoría");
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

            var result = await _categoriaApiService.GetByIdAsync(id);

            if (ErrorHelper.IsSuccess(result, out string? errorMessage) && result.Data != null)
            {
                var categoria = result.Data;
                var updateDto = new UpdateCategoriaDTO
                {
                    Id = categoria.Id,
                    Nombre = categoria.Nombre,
                    Descripcion = categoria.Descripcion,
                    Estado = categoria.Estado
                };

                TempData["Success"] = result.Message;
                return View(updateDto);
            }

            TempData["Error"] = errorMessage;
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateCategoriaDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _categoriaApiService.UpdateAsync(model);

            if (ErrorHelper.IsSuccess(result, out string? errorMessage))
            {
                TempData["Success"] = result.Message ?? SuccessMessages.GetUpdatedMessage("Categoría");
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

            var result = await _categoriaApiService.GetByIdAsync(id);

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

            var result = await _categoriaApiService.DeleteAsync(id);

            if (ErrorHelper.IsSuccess(result, out string? errorMessage))
            {
                return Json(new { success = true, message = result.Message ?? SuccessMessages.GetDeletedMessage("Categoría"), data = result.Data });
            }

            return Json(new { success = false, message = errorMessage });
        }
    }
}
