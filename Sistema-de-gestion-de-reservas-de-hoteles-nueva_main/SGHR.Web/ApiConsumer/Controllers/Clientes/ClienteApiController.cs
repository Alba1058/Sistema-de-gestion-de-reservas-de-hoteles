using Microsoft.AspNetCore.Mvc;
using SGHR.Application.DTOs.Clientes.Cliente;
using SGHR.Domain.Base;
using SGHR.Web.Infrastructure.Services.Api.Interfaces;

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

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> _List()
        {
            var result = await _clienteApiService.GetAllAsync();

            if (result != null && result.Success)
            {
                TempData["Success"] = result.Message;
                return PartialView("_List", result.Data ?? new List<ClienteDTO>());
            }

            TempData["Error"] = result?.Message ?? "Error al obtener los clientes";
            return PartialView("_List", new List<ClienteDTO>());
        }

        public async Task<IActionResult> Details(int id)
        {
            var result = await _clienteApiService.GetByIdAsync(id);

            if (result != null && result.Success && result.Data != null)
            {
                TempData["Success"] = result.Message;
                return View(result.Data);
            }

            TempData["Error"] = result?.Message ?? "Error desconocido al obtener detalles.";
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
            _logger.LogInformation("ClienteApiController.Create POST: Iniciando. Model recibido - Nombre: {Nombre}, Email: {Email}", model?.Nombre, model?.Email);

            if (!ModelState.IsValid)
            {
                var modelErrors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                
                _logger.LogWarning("ClienteApiController.Create POST: ModelState inválido. Errores: {Errors}", string.Join("; ", modelErrors));
                
                TempData["Error"] = $"Errores de validación: {string.Join("; ", modelErrors)}";
                return View(model);
            }

            if (model == null)
            {
                _logger.LogError("ClienteApiController.Create POST: El modelo es null");
                TempData["Error"] = "El modelo recibido es nulo";
                return View(new ClienteCreateDTO());
            }

            var result = await _clienteApiService.CreateAsync(model);

            if (result != null && result.Success)
            {
                _logger.LogInformation("ClienteApiController.Create POST: Cliente creado exitosamente - ID: {Id}", result.Data?.Id);
                TempData["Success"] = result.Message ?? "Cliente creado exitosamente";
                return RedirectToAction("Index");
            }

            _logger.LogWarning("ClienteApiController.Create POST: Error al crear cliente - {Message}", result?.Message);
            TempData["Error"] = result?.Message ?? "Error al crear el cliente";
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var result = await _clienteApiService.GetByIdAsync(id);

            if (result != null && result.Success && result.Data != null)
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

            TempData["Error"] = result?.Message ?? "Error desconocido al preparar la edición.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ClienteUpdateDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _clienteApiService.UpdateAsync(model);

            if (result != null && result.Success)
            {
                TempData["Success"] = result.Message ?? "Cliente actualizado exitosamente";
                return RedirectToAction("Index");
            }

            TempData["Error"] = result?.Message ?? "Error al actualizar el cliente";
            return View(model);
        }

        public async Task<IActionResult> _Delete(int id)
        {
            var result = await _clienteApiService.GetByIdAsync(id);

            if (result != null && result.Success && result.Data != null)
            {
                TempData["Success"] = result.Message;
                return PartialView("_Delete", result.Data);
            }

            TempData["Error"] = result?.Message ?? "Error desconocido al obtener el cliente para eliminar.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ActionName("_DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _DeleteConfirmed(int id)
        {
            var result = await _clienteApiService.DeleteAsync(id);

            if (result != null && result.Success)
            {
                return Json(new { success = true, message = result.Message, data = result.Data });
            }

            return Json(new { success = false, message = result?.Message ?? "Error desconocido al confirmar la eliminación" });
        }

        private async Task<List<ClienteDTO>> GetClientesAsync()
        {
            var result = await _clienteApiService.GetAllAsync();

            if (result != null && result.Success)
            {
                TempData["Success"] = result.Message;
                return result.Data ?? new List<ClienteDTO>();
            }

            TempData["Error"] = result?.Message ?? "Error al obtener los clientes";
            return new List<ClienteDTO>();
        }
    }
}
