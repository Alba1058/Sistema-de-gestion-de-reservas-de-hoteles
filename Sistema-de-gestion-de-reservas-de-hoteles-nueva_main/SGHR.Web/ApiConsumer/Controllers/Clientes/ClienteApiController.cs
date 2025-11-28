using Microsoft.AspNetCore.Mvc;
using SGHR.Application.DTOs.Clientes.Cliente;
using SGHR.Domain.Base;
using SGHR.Web.Infrastructure.Services.Api.Interfaces;
using SGHR.Web.Helpers;

namespace SGHR.Web.ApiConsumer.Controllers.Clientes
{
    public class ClienteApiController : Controller
    {
        private readonly ILogger<ClienteApiController> _logger;
        private readonly IClienteApiService _clienteApiService;

        public ClienteApiController(
            IClienteApiService clienteApiService,
            ILogger<ClienteApiController> logger)
        {
            _clienteApiService = clienteApiService ?? throw new ArgumentNullException(nameof(clienteApiService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var result = await _clienteApiService.GetAllAsync();
                var clientes = result?.Data ?? new List<ClienteDTO>();
                return View(clientes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de clientes");
                TempData["Error"] = "Error al cargar los clientes";
                return View(new List<ClienteDTO>());
            }
        }

        public async Task<IActionResult> _List()
        {
            try
            {
                var result = await _clienteApiService.GetAllAsync();

                if (ErrorHelper.IsSuccess(result, out string? errorMessage))
                {
                    TempData["Success"] = result.Message ?? SuccessMessages.Loaded;
                    return PartialView("_List", result.Data ?? new List<ClienteDTO>());
                }

                TempData["Error"] = errorMessage;
                return PartialView("_List", new List<ClienteDTO>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de clientes (partial)");
                TempData["Error"] = "Error al cargar los clientes";
                return PartialView("_List", new List<ClienteDTO>());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            if (!ErrorHelper.IsValidId(id, out string? idError))
            {
                TempData["Error"] = idError;
                return RedirectToAction("Index");
            }

            var result = await _clienteApiService.GetByIdAsync(id);

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
            var model = new ClienteCreateDTO();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClienteCreateDTO model)
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
                return View(new ClienteCreateDTO());
            }

            var result = await _clienteApiService.CreateAsync(model);

            if (ErrorHelper.IsSuccess(result, out string? errorMessage))
            {
                TempData["Success"] = result.Message ?? SuccessMessages.GetCreatedMessage("Cliente");
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

            var result = await _clienteApiService.GetByIdAsync(id);

            if (ErrorHelper.IsSuccess(result, out string? errorMessage) && result.Data != null)
            {
                var cliente = result.Data;
                var updateDto = new ClienteUpdateDTO
                {
                    Id = cliente.Id,
                    Nombre = cliente.Nombre,
                    Apellido = cliente.Apellido,
                    Email = cliente.Email,
                    Telefono = cliente.Telefono,
                    Direccion = cliente.Direccion,
                    Identificacion = cliente.Identificacion
                };

                TempData["Success"] = result.Message;
                return View(updateDto);
            }

            TempData["Error"] = errorMessage;
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ClienteUpdateDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _clienteApiService.UpdateAsync(model);

            if (ErrorHelper.IsSuccess(result, out string? errorMessage))
            {
                TempData["Success"] = result.Message ?? SuccessMessages.GetUpdatedMessage("Cliente");
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

            var result = await _clienteApiService.GetByIdAsync(id);

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

            var result = await _clienteApiService.DeleteAsync(id);

            if (ErrorHelper.IsSuccess(result, out string? errorMessage))
            {
                return Json(new { success = true, message = result.Message ?? SuccessMessages.GetDeletedMessage("Cliente"), data = result.Data });
            }

            return Json(new { success = false, message = errorMessage });
        }

        private async Task<List<ClienteDTO>> GetClientesAsync()
        {
            var result = await _clienteApiService.GetAllAsync();

            if (ErrorHelper.IsSuccess(result, out string? errorMessage))
            {
                TempData["Success"] = result.Message ?? SuccessMessages.Loaded;
                return result.Data ?? new List<ClienteDTO>();
            }

            TempData["Error"] = errorMessage;
            return new List<ClienteDTO>();
        }
    }
}
