using Microsoft.AspNetCore.Mvc;
using SGHR.Application.DTOs.Reservas.Reserva;
using SGHR.Domain.Base;
using SGHR.Web.Infrastructure.Services.Api.Facade;
using SGHR.Web.Infrastructure.Services.Api.Interfaces;
using SGHR.Web.ViewModels.Reservas;
using SGHR.Web.Helpers;
using System.Net.Http;

namespace SGHR.Web.ApiConsumer.Controllers.Reservas
{
    public class ReservaApiController : Controller
    {
        private readonly ILogger<ReservaApiController> _logger;
        private readonly IReservaApiService _reservaApiService;
        private readonly IReservaApiFacade _reservaApiFacade;

        public ReservaApiController(
            IReservaApiService reservaApiService,
            IReservaApiFacade reservaApiFacade,
            ILogger<ReservaApiController> logger)
        {
            _reservaApiService = reservaApiService ?? throw new ArgumentNullException(nameof(reservaApiService));
            _reservaApiFacade = reservaApiFacade ?? throw new ArgumentNullException(nameof(reservaApiFacade));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var result = await _reservaApiService.GetAllAsync();
                var reservas = result?.Data ?? new List<ReservaDTO>();
                return View(reservas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de reservas");
                TempData["Error"] = "Error al cargar las reservas";
                return View(new List<ReservaDTO>());
            }
        }

        public async Task<IActionResult> _List()
        {
            try
            {
                var result = await _reservaApiService.GetAllAsync();

                if (ErrorHelper.IsSuccess(result, out string? errorMessage))
                {
                    TempData["Success"] = result.Message ?? SuccessMessages.Loaded;
                    return PartialView("_List", result.Data ?? new List<ReservaDTO>());
                }

                TempData["Error"] = errorMessage;
                return PartialView("_List", new List<ReservaDTO>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de reservas (partial)");
                TempData["Error"] = "Error al cargar las reservas";
                return PartialView("_List", new List<ReservaDTO>());
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
                var result = await _reservaApiService.GetByIdAsync(id);

                if (ErrorHelper.IsSuccess(result, out string? errorMessage) && result.Data != null)
                {
                    return View(result.Data);
                }

                TempData["Error"] = errorMessage;
                return RedirectToAction("Index");
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("404"))
            {
                _logger.LogWarning(ex, "Reserva no encontrada - ID: {Id}", id);
                TempData["Error"] = "La reserva solicitada no fue encontrada";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener detalles de reserva - ID: {Id}", id);
                TempData["Error"] = "Error al cargar los detalles de la reserva";
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Create()
        {
            try
            {
                TempData.Remove("Success");
                TempData.Remove("Error");

                var viewModel = await _reservaApiFacade.GetCreateReservaDataAsync();
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar datos para crear reserva");
                TempData["Error"] = "Error al cargar los datos necesarios";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateReservaViewModel viewModel)
        {
            try
            {
                if (viewModel?.Reserva == null)
                {
                    _logger.LogError("El modelo recibido es null");
                    TempData["Error"] = "El modelo recibido es nulo";
                    return await Create();
                }

                var model = viewModel.Reserva;

                if (model.FechaInicio == default(DateTime) || model.FechaInicio < DateTime.Now.AddDays(-1))
                {
                    ModelState.AddModelError("Reserva.FechaInicio", "La fecha de inicio debe ser válida y no puede ser anterior a hoy.");
                }

                if (model.FechaFin == default(DateTime) || model.FechaFin < model.FechaInicio)
                {
                    ModelState.AddModelError("Reserva.FechaFin", "La fecha de fin debe ser válida y posterior a la fecha de inicio.");
                }

                if (!ModelState.IsValid)
                {
                    viewModel = await _reservaApiFacade.GetCreateReservaDataAsync();
                    viewModel.Reserva = model;
                    return View(viewModel);
                }

                var result = await _reservaApiService.CreateAsync(model);

                if (ErrorHelper.IsSuccess(result, out string? errorMessage))
                {
                    TempData["Success"] = result.Message ?? SuccessMessages.GetCreatedMessage("Reserva");
                    return RedirectToAction("Index");
                }

                TempData["Error"] = errorMessage;
                viewModel = await _reservaApiFacade.GetCreateReservaDataAsync();
                viewModel.Reserva = model;
                return View(viewModel);
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("404"))
            {
                _logger.LogError(ex, "Recurso no encontrado al crear reserva");
                TempData["Error"] = "El recurso solicitado no fue encontrado";
                return await Create();
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("500"))
            {
                _logger.LogError(ex, "Error del servidor al crear reserva");
                TempData["Error"] = "Error interno del servidor. Por favor, intente más tarde";
                return await Create();
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout al crear reserva");
                TempData["Error"] = "La operación tardó demasiado. Por favor, intente nuevamente";
                return await Create();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al crear reserva");
                TempData["Error"] = "Ocurrió un error inesperado. Por favor, contacte al administrador";
                return await Create();
            }
        }


        public async Task<IActionResult> Edit(int id)
        {
            if (!ErrorHelper.IsValidId(id, out string? idError))
            {
                TempData["Error"] = idError;
                return RedirectToAction("Index");
            }

            try
            {
                // verificacion de que la reserva existe
                var reservaResult = await _reservaApiService.GetByIdAsync(id);
                
                if (!ErrorHelper.IsSuccess(reservaResult, out string? errorMessage) || reservaResult.Data == null)
                {
                    _logger.LogWarning("Reserva no encontrada para editar - ID: {Id}, Error: {Error}", id, errorMessage);
                    TempData["Error"] = errorMessage ?? "La reserva solicitada no fue encontrada";
                    return RedirectToAction("Index");
                }

                var viewModel = await _reservaApiFacade.GetEditReservaDataAsync(id);

                if (viewModel?.Reserva == null || viewModel.Reserva.Id == 0)
                {
                    _logger.LogWarning("Error al cargar ViewModel para editar reserva - ID: {Id}. La reserva existe pero no se pudo cargar el ViewModel.", id);
                    TempData["Error"] = "Error al cargar los datos para editar. Por favor, intente nuevamente.";
                    return RedirectToAction("Index");
                }

                return View(viewModel);
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("404"))
            {
                _logger.LogWarning(ex, "Reserva no encontrada para editar - ID: {Id}", id);
                TempData["Error"] = "La reserva solicitada no fue encontrada";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al preparar edición de reserva - ID: {Id}", id);
                TempData["Error"] = "Error al cargar los datos para editar";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditReservaViewModel viewModel)
        {
            try
            {
                if (viewModel?.Reserva == null)
                {
                    _logger.LogError("El modelo recibido es null");
                    TempData["Error"] = "El modelo recibido es nulo";
                    return RedirectToAction("Index");
                }

                var model = viewModel.Reserva;

                if (!ErrorHelper.IsValidId(model.Id, out string? idError))
                {
                    TempData["Error"] = idError;
                    return RedirectToAction("Index");
                }

                if (model.FechaInicio == default(DateTime) || model.FechaInicio < DateTime.Now.AddDays(-1))
                {
                    ModelState.AddModelError("Reserva.FechaInicio", "La fecha de inicio debe ser válida y no puede ser anterior a hoy.");
                }

                if (model.FechaFin == default(DateTime) || model.FechaFin < model.FechaInicio)
                {
                    ModelState.AddModelError("Reserva.FechaFin", "La fecha de fin debe ser válida y posterior a la fecha de inicio.");
                }

                if (!ModelState.IsValid)
                {
                    var editViewModelInvalid = await _reservaApiFacade.GetEditReservaDataAsync(model.Id);
                    editViewModelInvalid.Reserva = model;
                    return View(editViewModelInvalid);
                }


                var result = await _reservaApiService.UpdateAsync(model);

                if (ErrorHelper.IsSuccess(result, out string? errorMessage))
                {
                    TempData["Success"] = result.Message ?? SuccessMessages.GetUpdatedMessage("Reserva");
                    return RedirectToAction("Index");
                }

                TempData["Error"] = errorMessage;
                
                var editViewModelError = await _reservaApiFacade.GetEditReservaDataAsync(model.Id);
                editViewModelError.Reserva = model;
                return View(editViewModelError);
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("404"))
            {
                _logger.LogError(ex, "Recurso no encontrado al editar reserva");
                TempData["Error"] = "El recurso solicitado no fue encontrado";
                return RedirectToAction("Index");
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("500"))
            {
                _logger.LogError(ex, "Error del servidor al editar reserva");
                TempData["Error"] = "Error interno del servidor. Por favor, intente más tarde";
                return RedirectToAction("Index");
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout al editar reserva");
                TempData["Error"] = "La operación tardó demasiado. Por favor, intente nuevamente";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al editar reserva");
                TempData["Error"] = "Ocurrió un error inesperado. Por favor, contacte al administrador";
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> _Delete(int id)
        {
            if (!ErrorHelper.IsValidId(id, out string? idError))
            {
                TempData["Error"] = idError;
                return RedirectToAction("Index");
            }

            try
            {
                var result = await _reservaApiService.GetByIdAsync(id);

                if (ErrorHelper.IsSuccess(result, out string? errorMessage) && result.Data != null)
                {
                    TempData["Success"] = result.Message;
                    return PartialView("_Delete", result.Data);
                }

                TempData["Error"] = errorMessage;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener reserva para eliminar - ID: {Id}", id);
                TempData["Error"] = "Error al cargar los datos para eliminar";
                return RedirectToAction("Index");
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
                var result = await _reservaApiService.DeleteAsync(id);

                if (ErrorHelper.IsSuccess(result, out string? errorMessage))
                {
                    return Json(new { success = true, message = result.Message ?? SuccessMessages.GetDeletedMessage("Reserva"), data = result.Data });
                }

                return Json(new { success = false, message = errorMessage });
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("404"))
            {
                _logger.LogWarning(ex, "Reserva no encontrada para eliminar - ID: {Id}", id);
                return Json(new { success = false, message = "La reserva solicitada no fue encontrada" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar reserva - ID: {Id}", id);
                return Json(new { success = false, message = "Ocurrió un error inesperado al eliminar la reserva" });
            }
        }
    }
}
