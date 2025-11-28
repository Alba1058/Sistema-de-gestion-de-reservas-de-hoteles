using Microsoft.AspNetCore.Mvc;
using SGHR.Application.DTOs.Reservas.Habitacion;
using SGHR.Domain.Base;
using SGHR.Web.Infrastructure.Services.Api.Interfaces;
using SGHR.Web.ViewModels.Reservas;
using SGHR.Web.Helpers;
using System.Net.Http;
using SGHR.Application.DTOs.Configuration.Categoria;
using SGHR.Application.DTOs.Configuration.Piso;

namespace SGHR.Web.ApiConsumer.Controllers.Reservas
{
    public class HabitacionApiController : Controller
    {
        private readonly ILogger<HabitacionApiController> _logger;
        private readonly IHabitacionApiService _habitacionApiService;
        private readonly ICategoriaApiService _categoriaApiService;
        private readonly IPisoApiService _pisoApiService;

        public HabitacionApiController(
            IHabitacionApiService habitacionApiService,
            ICategoriaApiService categoriaApiService,
            IPisoApiService pisoApiService,
            ILogger<HabitacionApiController> logger)
        {
            _habitacionApiService = habitacionApiService ?? throw new ArgumentNullException(nameof(habitacionApiService));
            _categoriaApiService = categoriaApiService ?? throw new ArgumentNullException(nameof(categoriaApiService));
            _pisoApiService = pisoApiService ?? throw new ArgumentNullException(nameof(pisoApiService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var result = await _habitacionApiService.GetAllAsync();
                var habitaciones = result?.Data ?? new List<HabitacionDTO>();
                return View(habitaciones);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de habitaciones");
                TempData["Error"] = "Error al cargar las habitaciones";
                return View(new List<HabitacionDTO>());
            }
        }

        public async Task<IActionResult> _List()
        {
            try
            {
                var result = await _habitacionApiService.GetAllAsync();

                if (ErrorHelper.IsSuccess(result, out string? errorMessage))
                {
                    TempData["Success"] = result.Message ?? SuccessMessages.Loaded;
                    return PartialView("_List", result.Data ?? new List<HabitacionDTO>());
                }

                TempData["Error"] = errorMessage;
                return PartialView("_List", new List<HabitacionDTO>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de habitaciones (partial)");
                TempData["Error"] = "Error al cargar las habitaciones";
                return PartialView("_List", new List<HabitacionDTO>());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            if (!ErrorHelper.IsValidId(id, out string? idError))
            {
                TempData["Error"] = idError;
                return RedirectToAction("Index");
            }

            try
            {
                var result = await _habitacionApiService.GetByIdAsync(id);

                if (ErrorHelper.IsSuccess(result, out string? errorMessage) && result.Data != null)
                {
                    TempData["Success"] = result.Message;
                    return View(result.Data);
                }

                TempData["Error"] = errorMessage;
                return RedirectToAction("Index");
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("404"))
            {
                _logger.LogWarning(ex, "Habitación no encontrada - ID: {Id}", id);
                TempData["Error"] = "La habitación solicitada no fue encontrada";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener detalles de habitación - ID: {Id}", id);
                TempData["Error"] = "Error al cargar los detalles de la habitación";
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Create()
        {
            try
            {
                TempData.Remove("Success");
                TempData.Remove("Error");

                var categoriasResult = await _categoriaApiService.GetAllAsync();
                var pisosResult = await _pisoApiService.GetAllAsync();

                var viewModel = new CreateHabitacionViewModel
                {
                    Categorias = categoriasResult?.Data ?? new(),
                    Pisos = pisosResult?.Data ?? new()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar datos para crear habitación");
                TempData["Error"] = "Error al cargar los datos necesarios";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateHabitacionViewModel viewModel)
        {
            try
            {
                if (viewModel?.Habitacion == null)
                {
                    _logger.LogError("El modelo recibido es null");
                    TempData["Error"] = "El modelo recibido es nulo";
                    return await Create();
                }

                var model = viewModel.Habitacion;

                if (!ModelState.IsValid)
                {
                    var categoriasResult = await _categoriaApiService.GetAllAsync();
                    var pisosResult = await _pisoApiService.GetAllAsync();
                    viewModel.Categorias = categoriasResult?.Data ?? new();
                    viewModel.Pisos = pisosResult?.Data ?? new();
                    return View(viewModel);
                }


                var result = await _habitacionApiService.CreateAsync(model);

                if (ErrorHelper.IsSuccess(result, out string? errorMessage))
                {
                    TempData["Success"] = result.Message ?? SuccessMessages.GetCreatedMessage("Habitación");
                    return RedirectToAction("Index");
                }

                TempData["Error"] = errorMessage;
                
                var categoriasResult2 = await _categoriaApiService.GetAllAsync();
                var pisosResult2 = await _pisoApiService.GetAllAsync();
                viewModel.Categorias = categoriasResult2?.Data ?? new();
                viewModel.Pisos = pisosResult2?.Data ?? new();
                return View(viewModel);
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("404"))
            {
                _logger.LogError(ex, "Recurso no encontrado al crear habitación");
                TempData["Error"] = "El recurso solicitado no fue encontrado";
                return await Create();
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("500"))
            {
                _logger.LogError(ex, "Error del servidor al crear habitación");
                TempData["Error"] = "Error interno del servidor. Por favor, intente más tarde";
                return await Create();
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout al crear habitación");
                TempData["Error"] = "La operación tardó demasiado. Por favor, intente nuevamente";
                return await Create();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear habitación");
                TempData["Error"] = "Ocurrió un error inesperado. Por favor, contacte al administrador";
                return await Create();
            }
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!ErrorHelper.IsValidId(id, out string? idError))
            {
                TempData["Error"] = idError;
                return RedirectToAction("Index");
            }

            try
            {
                var result = await _habitacionApiService.GetByIdAsync(id);

                if (ErrorHelper.IsSuccess(result, out string? errorMessage) && result.Data != null)
                {
                    var habitacion = result.Data;
                    
                    var categoriasResult = await _categoriaApiService.GetAllAsync();
                    var pisosResult = await _pisoApiService.GetAllAsync();

                    var viewModel = new EditHabitacionViewModel
                    {
                        Habitacion = new UpdateHabitacionDTO
                        {
                            Id = habitacion.Id,
                            Numero = habitacion.Numero,
                            IdCategoria = habitacion.IdCategoria,
                            IdPiso = habitacion.IdPiso,
                            EstadoHabitacion = habitacion.EstadoHabitacion,
                            PrecioBase = habitacion.PrecioBase,
                            Descripcion = habitacion.Descripcion,
                            Estado = habitacion.Estado
                        },
                        Categorias = categoriasResult?.Data ?? new(),
                        Pisos = pisosResult?.Data ?? new()
                    };

                    TempData["Success"] = result.Message;
                    return View(viewModel);
                }

                TempData["Error"] = errorMessage;
                return RedirectToAction("Index");
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("404"))
            {
                _logger.LogWarning(ex, "Habitación no encontrada para editar - ID: {Id}", id);
                TempData["Error"] = "La habitación solicitada no fue encontrada";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al preparar edición de habitación - ID: {Id}", id);
                TempData["Error"] = "Error al cargar los datos para editar";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditHabitacionViewModel viewModel)
        {
            try
            {
                if (viewModel?.Habitacion == null)
                {
                    _logger.LogError("El modelo recibido es null");
                    TempData["Error"] = "El modelo recibido es nulo";
                    return RedirectToAction("Index");
                }

                var model = viewModel.Habitacion;

                if (!ErrorHelper.IsValidId(model.Id, out string? idError))
                {
                    TempData["Error"] = idError;
                    return RedirectToAction("Index");
                }

                if (!ModelState.IsValid)
                {
                    var categoriasResult = await _categoriaApiService.GetAllAsync();
                    var pisosResult = await _pisoApiService.GetAllAsync();
                    viewModel.Categorias = categoriasResult?.Data ?? new();
                    viewModel.Pisos = pisosResult?.Data ?? new();
                    return View(viewModel);
                }


                var result = await _habitacionApiService.UpdateAsync(model);

                if (ErrorHelper.IsSuccess(result, out string? errorMessage))
                {
                    TempData["Success"] = result.Message ?? SuccessMessages.GetUpdatedMessage("Habitación");
                    return RedirectToAction("Index");
                }

                TempData["Error"] = errorMessage;
                
                var categoriasResult2 = await _categoriaApiService.GetAllAsync();
                var pisosResult2 = await _pisoApiService.GetAllAsync();
                viewModel.Categorias = categoriasResult2?.Data ?? new();
                viewModel.Pisos = pisosResult2?.Data ?? new();
                return View(viewModel);
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("404"))
            {
                _logger.LogError(ex, "Recurso no encontrado al editar habitación");
                TempData["Error"] = "El recurso solicitado no fue encontrado";
                return RedirectToAction("Index");
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("500"))
            {
                _logger.LogError(ex, "Error del servidor al editar habitación");
                TempData["Error"] = "Error interno del servidor. Por favor, intente más tarde";
                return RedirectToAction("Index");
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout al editar habitación");
                TempData["Error"] = "La operación tardó demasiado. Por favor, intente nuevamente";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al editar habitación");
                TempData["Error"] = "Ocurrió un error inesperado. Por favor, contacte al administrador";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> _Delete(int id)
        {
            if (!ErrorHelper.IsValidId(id, out string? idError))
            {
                return PartialView("_Delete", (HabitacionDTO?)null);
            }

            try
            {
                var result = await _habitacionApiService.GetByIdAsync(id);

                if (ErrorHelper.IsSuccess(result, out string? errorMessage) && result.Data != null)
                {
                    return PartialView("_Delete", result.Data);
                }

                return PartialView("_Delete", (HabitacionDTO?)null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener habitación para eliminar - ID: {Id}", id);
                return PartialView("_Delete", (HabitacionDTO?)null);
            }
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

            try
            {
                var result = await _habitacionApiService.DeleteAsync(id);

                if (ErrorHelper.IsSuccess(result, out string? errorMessage))
                {
                    return Json(new { success = true, message = result.Message ?? SuccessMessages.GetDeletedMessage("Habitación"), data = result.Data });
                }

                return Json(new { success = false, message = errorMessage });
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("404"))
            {
                _logger.LogWarning(ex, "Habitación no encontrada para eliminar - ID: {Id}", id);
                return Json(new { success = false, message = "La habitación solicitada no fue encontrada" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar habitación - ID: {Id}", id);
                return Json(new { success = false, message = "Ocurrió un error inesperado al eliminar la habitación" });
            }
        }
    }
}
