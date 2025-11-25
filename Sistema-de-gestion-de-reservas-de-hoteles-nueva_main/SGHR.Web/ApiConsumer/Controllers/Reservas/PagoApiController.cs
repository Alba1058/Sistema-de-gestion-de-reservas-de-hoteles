using Microsoft.AspNetCore.Mvc;
using SGHR.Application.DTOs.Reservas.Pago;
using SGHR.Domain.Base;
using SGHR.Web.Infrastructure.HttpClients;
using System.Text.Json;

namespace SGHR.Web.ApiConsumer.Controllers.Reservas
{
    public class PagoApiController : Controller
    {
        private readonly ILogger<PagoApiController> _logger;
        private readonly PagoHttpClient _pagoClient = new PagoHttpClient();
        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public PagoApiController(ILogger<PagoApiController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var pagos = await GetPagosAsync();
            return View(pagos);
        }

        public async Task<IActionResult> _List()
        {
            var pagos = await GetPagosAsync();
            return PartialView("_List", pagos);
        }

        public async Task<IActionResult> Details(int id)
        {
            OperationResult<PagoDTO> result = null;
            try
            {
                using (_pagoClient.client)
                {
                    var response = await _pagoClient.Details(id);

                    if (!response.IsSuccessStatusCode)
                    {
                        TempData["Error"] = $"Error al consumir la API: {response.StatusCode}";
                        return RedirectToAction("Index");
                    }

                    var content = await response.Content.ReadAsStringAsync();
                    result = JsonSerializer.Deserialize<OperationResult<PagoDTO>>(content, _jsonSerializerOptions);

                    if (result != null && result.Success && result.Data != null)
                    {
                        return View(result.Data);
                    }
                    else
                    {
                        TempData["Error"] = result?.Message ?? "Error desconocido al obtener detalles.";
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al consumir la API: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        public IActionResult Create()
        {
            // Limpiar TempData de operaciones anteriores
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

            OperationResult<PagoDTO> resultPago = null;
            try
            {
                using (_pagoClient.client)
                {
                    var json = JsonSerializer.Serialize(model);
                    _logger.LogInformation($"PagoApiController.Create POST: Iniciando creación de pago. ModelState.IsValid: {ModelState.IsValid}");
                    _logger.LogInformation($"PagoApiController.Create POST: JSON enviado a la API: {json}");
                    
                    var response = await _pagoClient.Create(model);
                    var responseContent = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation($"PagoApiController.Create POST: Respuesta de la API (StatusCode: {response.StatusCode}). Contenido: {responseContent}");

                    if (!response.IsSuccessStatusCode)
                    {
                        try
                        {
                            var errorResult = JsonSerializer.Deserialize<OperationResult<PagoDTO>>(responseContent, _jsonSerializerOptions);
                            TempData["Error"] = errorResult?.Message ?? $"Error al crear el pago (Código: {response.StatusCode}). Respuesta: {responseContent}";
                        }
                        catch (JsonException jex)
                        {
                            TempData["Error"] = $"Error al crear el pago (Código: {response.StatusCode}). No se pudo deserializar la respuesta de error. Detalles: {responseContent}. Excepción: {jex.Message}";
                        }
                        catch
                        {
                            TempData["Error"] = $"Error al crear el pago (Código: {response.StatusCode}). Respuesta: {responseContent}";
                        }
                        return View(model);
                    }

                    resultPago = JsonSerializer.Deserialize<OperationResult<PagoDTO>>(responseContent, _jsonSerializerOptions);

                    if (resultPago != null && resultPago.Success)
                    {
                        TempData["Success"] = resultPago.Message ?? "Pago creado exitosamente";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["Error"] = resultPago?.Message ?? "Error al crear el pago";
                        return View(model);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al consumir la API: {ex.Message}";
                return View(model);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            OperationResult<PagoDTO> resultPago = null;
            try
            {
                using (_pagoClient.client)
                {
                    var response = await _pagoClient.Details(id);

                    if (!response.IsSuccessStatusCode)
                    {
                        TempData["Error"] = $"Error al consumir la API: {response.StatusCode}";
                        return RedirectToAction("Index");
                    }

                    var content = await response.Content.ReadAsStringAsync();
                    resultPago = JsonSerializer.Deserialize<OperationResult<PagoDTO>>(content, _jsonSerializerOptions);

                    if (resultPago != null && resultPago.Success && resultPago.Data != null)
                    {
                        var pago = resultPago.Data;
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
                    else
                    {
                        TempData["Error"] = resultPago?.Message ?? "Error desconocido al preparar la edición.";
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al consumir la API: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdatePagoDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            OperationResult<PagoDTO> resultPago = null;
            try
            {
                using (_pagoClient.client)
                {
                    var json = JsonSerializer.Serialize(model);
                    _logger.LogInformation($"PagoApiController.Edit POST: Iniciando edición de pago. ModelState.IsValid: {ModelState.IsValid}");
                    _logger.LogInformation($"PagoApiController.Edit POST: JSON enviado a la API: {json}");
                    
                    var response = await _pagoClient.Edit(model);
                    var responseContent = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation($"PagoApiController.Edit POST: Respuesta de la API (StatusCode: {response.StatusCode}). Contenido: {responseContent}");

                    if (!response.IsSuccessStatusCode)
                    {
                        try
                        {
                            var errorResult = JsonSerializer.Deserialize<OperationResult<PagoDTO>>(responseContent, _jsonSerializerOptions);
                            TempData["Error"] = errorResult?.Message ?? $"Error al actualizar el pago (Código: {response.StatusCode}). Respuesta: {responseContent}";
                        }
                        catch (JsonException jex)
                        {
                            TempData["Error"] = $"Error al actualizar el pago (Código: {response.StatusCode}). No se pudo deserializar la respuesta de error. Detalles: {responseContent}. Excepción: {jex.Message}";
                        }
                        catch
                        {
                            TempData["Error"] = $"Error al actualizar el pago (Código: {response.StatusCode}). Respuesta: {responseContent}";
                        }
                        return View(model);
                    }

                    resultPago = JsonSerializer.Deserialize<OperationResult<PagoDTO>>(responseContent, _jsonSerializerOptions);

                    if (resultPago != null && resultPago.Success)
                    {
                        TempData["Success"] = resultPago.Message ?? "Pago actualizado exitosamente";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["Error"] = resultPago?.Message ?? "Error al actualizar el pago";
                        return View(model);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al consumir la API: {ex.Message}";
                return View(model);
            }
        }

        public async Task<IActionResult> _Delete(int id)
        {
            try
            {
                using (_pagoClient.client)
                {
                    var response = await _pagoClient.Details(id);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        var result = JsonSerializer.Deserialize<OperationResult<PagoDTO>>(responseString, _jsonSerializerOptions);

                        if (result != null && result.Success && result.Data != null)
                        {
                            return PartialView("_Delete", result.Data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el pago para eliminar");
            }

            return PartialView("_Delete", null);
        }

        [HttpPost]
        [ActionName("_DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _DeleteConfirmed(int id)
        {
            OperationResult<bool> result = null;
            try
            {
                using (_pagoClient.client)
                {
                    var response = await _pagoClient.Delete(id);
                    
                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        return Json(new { success = false, message = $"Error: {response.StatusCode}. Detalles: {errorContent}" });
                    }

                    var content = await response.Content.ReadAsStringAsync();
                    result = JsonSerializer.Deserialize<OperationResult<bool>>(content, _jsonSerializerOptions);

                    if (result != null && result.Success)
                    {
                        return Json(new { success = true, message = result.Message, data = result.Data });
                    }
                    else
                    {
                        return Json(new { success = false, message = result?.Message ?? "Error al eliminar el pago" });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al consumir la API: {ex.Message}" });
            }
        }

        private async Task<List<PagoDTO>> GetPagosAsync()
        {
            OperationResult<List<PagoDTO>> result = null;
            try
            {
                using (_pagoClient.client)
                {
                    var response = await _pagoClient.Index();

                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        result = JsonSerializer.Deserialize<OperationResult<List<PagoDTO>>>(responseString, _jsonSerializerOptions);

                        if (result != null && result.Success)
                        {
                            return result.Data ?? new List<PagoDTO>();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los pagos");
            }

            return new List<PagoDTO>();
        }
    }
}
