using Microsoft.AspNetCore.Mvc;
using SGHR.Application.DTOs.Reservas.Tarifa;
using SGHR.Domain.Base;
using System.Text;
using System.Text.Json;

namespace SGHR.Web.ApiConsumer.Controllers.Reservas
{
    public class TarifaApiController : Controller
    {
        private readonly ILogger<TarifaApiController> _logger;
        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        private const string BaseApiAddress = "http://localhost:5066/api/";

        public TarifaApiController(ILogger<TarifaApiController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var tarifas = await GetTarifasAsync();
            return View(tarifas);
        }

        public async Task<IActionResult> _List()
        {
            var tarifas = await GetTarifasAsync();
            return PartialView("_List", tarifas);
        }

        public async Task<IActionResult> Details(int id)
        {
            OperationResult<TarifaDTO> result = null;
            try
            {
                using (var httpclient = new HttpClient())
                {
                    httpclient.BaseAddress = new Uri(BaseApiAddress);

                    var endpoint = await httpclient.GetAsync($"Tarifa/{id}");

                    if (!endpoint.IsSuccessStatusCode)
                    {
                        TempData["Error"] = $"Error al consumir la API: {endpoint.StatusCode}";
                        return RedirectToAction("Index");
                    }

                    var content = await endpoint.Content.ReadAsStringAsync();
                    result = JsonSerializer.Deserialize<OperationResult<TarifaDTO>>(content, _jsonSerializerOptions);

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
            TempData.Remove("Success");
            TempData.Remove("Error");
            return View(new CreateTarifaDTO());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTarifaDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            OperationResult<TarifaDTO> resultTarifa = null;
            try
            {
                using (var httpclient = new HttpClient())
                {
                    httpclient.BaseAddress = new Uri(BaseApiAddress);

                    var json = JsonSerializer.Serialize(model);
                    _logger.LogInformation($"TarifaApiController.Create POST: Iniciando creación de tarifa. ModelState.IsValid: {ModelState.IsValid}");
                    _logger.LogInformation($"TarifaApiController.Create POST: JSON enviado a la API: {json}");
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var endpointCreate = await httpclient.PostAsync("Tarifa", content);

                    var responseContent = await endpointCreate.Content.ReadAsStringAsync();
                    _logger.LogInformation($"TarifaApiController.Create POST: Respuesta de la API (StatusCode: {endpointCreate.StatusCode}). Contenido: {responseContent}");

                    if (!endpointCreate.IsSuccessStatusCode)
                    {
                        try
                        {
                            var errorResult = JsonSerializer.Deserialize<OperationResult<TarifaDTO>>(responseContent, _jsonSerializerOptions);
                            TempData["Error"] = errorResult?.Message ?? $"Error al crear la tarifa (Código: {endpointCreate.StatusCode}). Respuesta: {responseContent}";
                        }
                        catch (JsonException jex)
                        {
                            TempData["Error"] = $"Error al crear la tarifa (Código: {endpointCreate.StatusCode}). No se pudo deserializar la respuesta de error. Detalles: {responseContent}. Excepción: {jex.Message}";
                        }
                        catch
                        {
                            TempData["Error"] = $"Error al crear la tarifa (Código: {endpointCreate.StatusCode}). Respuesta: {responseContent}";
                        }
                        return View(model);
                    }

                    resultTarifa = JsonSerializer.Deserialize<OperationResult<TarifaDTO>>(responseContent, _jsonSerializerOptions);

                    if (resultTarifa != null && resultTarifa.Success)
                    {
                        TempData["Success"] = resultTarifa.Message ?? "Tarifa creada exitosamente";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["Error"] = resultTarifa?.Message ?? "Error al crear la tarifa";
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
            OperationResult<TarifaDTO> resultTarifa = null;
            try
            {
                using (var httpclient = new HttpClient())
                {
                    httpclient.BaseAddress = new Uri(BaseApiAddress);

                    var endpoint = await httpclient.GetAsync($"Tarifa/{id}");

                    if (!endpoint.IsSuccessStatusCode)
                    {
                        TempData["Error"] = $"Error al consumir la API: {endpoint.StatusCode}";
                        return RedirectToAction("Index");
                    }

                    var content = await endpoint.Content.ReadAsStringAsync();
                    resultTarifa = JsonSerializer.Deserialize<OperationResult<TarifaDTO>>(content, _jsonSerializerOptions);

                    if (resultTarifa != null && resultTarifa.Success && resultTarifa.Data != null)
                    {
                        var tarifa = resultTarifa.Data;
                        var updateDto = new UpdateTarifaDTO
                        {
                            Id = tarifa.Id,
                            Tipo = tarifa.Tipo,
                            Monto = tarifa.Monto,
                            FechaInicio = tarifa.FechaInicio,
                            FechaFin = tarifa.FechaFin,
                            PrecioPorNoche = tarifa.PrecioPorNoche,
                            Descuento = tarifa.Descuento,
                            Descripcion = tarifa.Descripcion,
                            IdHabitacion = tarifa.IdHabitacion,
                            Estado = tarifa.Estado
                        };

                        return View(updateDto);
                    }
                    else
                    {
                        TempData["Error"] = resultTarifa?.Message ?? "Error desconocido al preparar la edición.";
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
        public async Task<IActionResult> Edit(UpdateTarifaDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            OperationResult<TarifaDTO> resultTarifa = null;
            try
            {
                using (var httpclient = new HttpClient())
                {
                    httpclient.BaseAddress = new Uri(BaseApiAddress);

                    var json = JsonSerializer.Serialize(model);
                    _logger.LogInformation($"TarifaApiController.Edit POST: Iniciando edición de tarifa. ModelState.IsValid: {ModelState.IsValid}");
                    _logger.LogInformation($"TarifaApiController.Edit POST: JSON enviado a la API: {json}");
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var endpointEdit = await httpclient.PutAsync("Tarifa", content);

                    var responseContent = await endpointEdit.Content.ReadAsStringAsync();
                    _logger.LogInformation($"TarifaApiController.Edit POST: Respuesta de la API (StatusCode: {endpointEdit.StatusCode}). Contenido: {responseContent}");

                    if (!endpointEdit.IsSuccessStatusCode)
                    {
                        try
                        {
                            var errorResult = JsonSerializer.Deserialize<OperationResult<TarifaDTO>>(responseContent, _jsonSerializerOptions);
                            TempData["Error"] = errorResult?.Message ?? $"Error al actualizar la tarifa (Código: {endpointEdit.StatusCode}). Respuesta: {responseContent}";
                        }
                        catch (JsonException jex)
                        {
                            TempData["Error"] = $"Error al actualizar la tarifa (Código: {endpointEdit.StatusCode}). No se pudo deserializar la respuesta de error. Detalles: {responseContent}. Excepción: {jex.Message}";
                        }
                        catch
                        {
                            TempData["Error"] = $"Error al actualizar la tarifa (Código: {endpointEdit.StatusCode}). Respuesta: {responseContent}";
                        }
                        return View(model);
                    }

                    resultTarifa = JsonSerializer.Deserialize<OperationResult<TarifaDTO>>(responseContent, _jsonSerializerOptions);

                    if (resultTarifa != null && resultTarifa.Success)
                    {
                        TempData["Success"] = resultTarifa.Message ?? "Tarifa actualizada exitosamente";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["Error"] = resultTarifa?.Message ?? "Error al actualizar la tarifa";
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
                using (var httpclient = new HttpClient())
                {
                    httpclient.BaseAddress = new Uri(BaseApiAddress);
                    var endpoint = await httpclient.GetAsync($"Tarifa/{id}");

                    if (endpoint.IsSuccessStatusCode)
                    {
                        var responseString = await endpoint.Content.ReadAsStringAsync();
                        var result = JsonSerializer.Deserialize<OperationResult<TarifaDTO>>(responseString, _jsonSerializerOptions);

                        if (result != null && result.Success && result.Data != null)
                        {
                            return PartialView("_Delete", result.Data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la tarifa para eliminar");
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
                using (var httpclient = new HttpClient())
                {
                    httpclient.BaseAddress = new Uri(BaseApiAddress);

                    var endpointRemove = await httpclient.DeleteAsync($"Tarifa/{id}");
                    
                    if (!endpointRemove.IsSuccessStatusCode)
                    {
                        var errorContent = await endpointRemove.Content.ReadAsStringAsync();
                        return Json(new { success = false, message = $"Error: {endpointRemove.StatusCode}. Detalles: {errorContent}" });
                    }

                    var content = await endpointRemove.Content.ReadAsStringAsync();
                    result = JsonSerializer.Deserialize<OperationResult<bool>>(content, _jsonSerializerOptions);

                    if (result != null && result.Success)
                    {
                        return Json(new { success = true, message = result.Message, data = result.Data });
                    }
                    else
                    {
                        return Json(new { success = false, message = result?.Message ?? "Error al eliminar la tarifa" });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al consumir la API: {ex.Message}" });
            }
        }

        private async Task<List<TarifaDTO>> GetTarifasAsync()
        {
            OperationResult<List<TarifaDTO>> result = null;
            try
            {
                using (var httpclient = new HttpClient())
                {
                    httpclient.BaseAddress = new Uri(BaseApiAddress);
                    var endpoint = await httpclient.GetAsync("Tarifa");

                    if (endpoint.IsSuccessStatusCode)
                    {
                        var responseString = await endpoint.Content.ReadAsStringAsync();
                        result = JsonSerializer.Deserialize<OperationResult<List<TarifaDTO>>>(responseString, _jsonSerializerOptions);

                        if (result != null && result.Success)
                        {
                            return result.Data ?? new List<TarifaDTO>();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las tarifas");
            }

            return new List<TarifaDTO>();
        }
    }
}
