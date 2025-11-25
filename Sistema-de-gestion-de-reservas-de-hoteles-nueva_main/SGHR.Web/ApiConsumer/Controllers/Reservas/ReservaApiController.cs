using Microsoft.AspNetCore.Mvc;
using SGHR.Application.DTOs.Reservas.Reserva;
using SGHR.Domain.Base;
using System.Text;
using System.Text.Json;

namespace SGHR.Web.ApiConsumer.Controllers.Reservas
{
    public class ReservaApiController : Controller
    {
        private readonly ILogger<ReservaApiController> _logger;
        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        private const string BaseApiAddress = "http://localhost:5066/api/";

        public ReservaApiController(ILogger<ReservaApiController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var reservas = await GetReservasAsync(suppressTempData: true);
            return View(reservas);
        }

        public async Task<IActionResult> _List()
        {
            OperationResult<List<ReservaDTO>> result = null;
            try
            {
                using (var httpclient = new HttpClient())
                {
                    httpclient.BaseAddress = new Uri(BaseApiAddress);
                    var endpoint = await httpclient.GetAsync("Reserva");
                        
                    if (endpoint.IsSuccessStatusCode)
                    {
                        var responseString = await endpoint.Content.ReadAsStringAsync();
                        result = JsonSerializer.Deserialize<OperationResult<List<ReservaDTO>>>(responseString, _jsonSerializerOptions);

                        if (result != null && result.Success)
                        {
                            return PartialView("_List", result.Data ?? new List<ReservaDTO>());
                        }
                        else
                        {
                            return PartialView("_List", new List<ReservaDTO>());
                        }
                    }
                    else
                    {
                        return PartialView("_List", new List<ReservaDTO>());
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al consumir la API: {ex.Message}";
                return PartialView("_List", new List<ReservaDTO>());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            OperationResult<ReservaDTO> result = null;
            try
            {
                using (var httpclient = new HttpClient())
                {
                    httpclient.BaseAddress = new Uri(BaseApiAddress);

                    var endpoint = await httpclient.GetAsync($"Reserva/{id}");

                    if (!endpoint.IsSuccessStatusCode)
                    {
                        TempData["Error"] = $"Error al consumir la API: {endpoint.StatusCode}";
                        return RedirectToAction("Index");
                    }

                    var content = await endpoint.Content.ReadAsStringAsync();
                    result = JsonSerializer.Deserialize<OperationResult<ReservaDTO>>(content, _jsonSerializerOptions);

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
            var model = new CreateReservaDTO();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateReservaDTO model)
        {
            if (model.FechaInicio == default(DateTime) || model.FechaInicio < DateTime.Now.AddDays(-1))
            {
                ModelState.AddModelError("FechaInicio", "La fecha de inicio debe ser válida y no puede ser anterior a hoy.");
            }

            if (model.FechaFin == default(DateTime) || model.FechaFin < model.FechaInicio)
            {
                ModelState.AddModelError("FechaFin", "La fecha de fin debe ser válida y posterior a la fecha de inicio.");
            }

            if (!ModelState.IsValid)
                return View(model);

            OperationResult<ReservaDTO> resultReserva = null;
            try
            {
                using (var httpclient = new HttpClient())
                {
                    httpclient.BaseAddress = new Uri(BaseApiAddress);

                    var json = JsonSerializer.Serialize(model);
                    _logger.LogInformation($"ReservaApiController.Create POST: Iniciando creación de reserva. ModelState.IsValid: {ModelState.IsValid}");
                    _logger.LogInformation($"ReservaApiController.Create POST: JSON enviado a la API: {json}");
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var endpointCreate = await httpclient.PostAsync("Reserva", content);

                    var responseContent = await endpointCreate.Content.ReadAsStringAsync();
                    _logger.LogInformation($"ReservaApiController.Create POST: Respuesta de la API (StatusCode: {endpointCreate.StatusCode}). Contenido: {responseContent}");

                    if (!endpointCreate.IsSuccessStatusCode)
                    {
                        try
                        {
                            var errorResult = JsonSerializer.Deserialize<OperationResult<ReservaDTO>>(responseContent, _jsonSerializerOptions);
                            TempData["Error"] = errorResult?.Message ?? $"Error al crear la reserva (Código: {endpointCreate.StatusCode}). Respuesta: {responseContent}";
                        }
                        catch (JsonException jex)
                        {
                            TempData["Error"] = $"Error al crear la reserva (Código: {endpointCreate.StatusCode}). No se pudo deserializar la respuesta de error. Detalles: {responseContent}. Excepción: {jex.Message}";
                        }
                        catch
                        {
                            TempData["Error"] = $"Error al crear la reserva (Código: {endpointCreate.StatusCode}). Respuesta: {responseContent}";
                        }
                        return View(model);
                    }

                    resultReserva = JsonSerializer.Deserialize<OperationResult<ReservaDTO>>(responseContent, _jsonSerializerOptions);

                    if (resultReserva != null && resultReserva.Success)
                    {
                        TempData["Success"] = resultReserva.Message ?? "Reserva creada exitosamente";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["Error"] = resultReserva?.Message ?? "Error al crear la reserva";
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
            OperationResult<ReservaDTO> resultReserva = null;
            try
            {
                using (var httpclient = new HttpClient())
                {
                    httpclient.BaseAddress = new Uri(BaseApiAddress);

                    var endpoint = await httpclient.GetAsync($"Reserva/{id}");

                    if (!endpoint.IsSuccessStatusCode)
                    {
                        TempData["Error"] = $"Error al consumir la API: {endpoint.StatusCode}";
                        return RedirectToAction("Index");
                    }

                    var content = await endpoint.Content.ReadAsStringAsync();
                    resultReserva = JsonSerializer.Deserialize<OperationResult<ReservaDTO>>(content, _jsonSerializerOptions);

                    if (resultReserva != null && resultReserva.Success && resultReserva.Data != null)
                    {
                        var reserva = resultReserva.Data;
                        var updateDto = new UpdateReservaDTO
                        {
                            Id = reserva.Id,
                            IdCliente = reserva.IdCliente,
                            IdHabitacion = reserva.IdHabitacion,
                            FechaInicio = reserva.FechaInicio,
                            FechaFin = reserva.FechaFin,
                            NumeroHuespedes = reserva.NumeroHuespedes,
                            Total = reserva.Total,
                            EstadoReserva = reserva.EstadoReserva,
                            Estado = reserva.Estado
                        };

                        return View(updateDto);
                    }
                    else
                    {
                        TempData["Error"] = resultReserva?.Message ?? "Error desconocido al preparar la edición.";
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
        public async Task<IActionResult> Edit(UpdateReservaDTO model)
        {
            if (model.FechaInicio == default(DateTime) || model.FechaInicio < DateTime.Now.AddDays(-1))
            {
                ModelState.AddModelError("FechaInicio", "La fecha de inicio debe ser válida y no puede ser anterior a hoy.");
            }

            if (model.FechaFin == default(DateTime) || model.FechaFin < model.FechaInicio)
            {
                ModelState.AddModelError("FechaFin", "La fecha de fin debe ser válida y posterior a la fecha de inicio.");
            }

            if (!ModelState.IsValid)
                return View(model);

            OperationResult<ReservaDTO> resultReserva = null;
            try
            {
                using (var httpclient = new HttpClient())
                {
                    httpclient.BaseAddress = new Uri(BaseApiAddress);

                    var json = JsonSerializer.Serialize(model);
                    _logger.LogInformation($"ReservaApiController.Edit POST: Iniciando edición de reserva. ModelState.IsValid: {ModelState.IsValid}");
                    _logger.LogInformation($"ReservaApiController.Edit POST: JSON enviado a la API: {json}");
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var endpointEdit = await httpclient.PutAsync("Reserva", content);

                    var responseContent = await endpointEdit.Content.ReadAsStringAsync();
                    _logger.LogInformation($"ReservaApiController.Edit POST: Respuesta de la API (StatusCode: {endpointEdit.StatusCode}). Contenido: {responseContent}");

                    if (!endpointEdit.IsSuccessStatusCode)
                    {
                        try
                        {
                            var errorResult = JsonSerializer.Deserialize<OperationResult<ReservaDTO>>(responseContent, _jsonSerializerOptions);
                            TempData["Error"] = errorResult?.Message ?? $"Error al actualizar la reserva (Código: {endpointEdit.StatusCode}). Respuesta: {responseContent}";
                        }
                        catch (JsonException jex)
                        {
                            TempData["Error"] = $"Error al actualizar la reserva (Código: {endpointEdit.StatusCode}). No se pudo deserializar la respuesta de error. Detalles: {responseContent}. Excepción: {jex.Message}";
                        }
                        catch
                        {
                            TempData["Error"] = $"Error al actualizar la reserva (Código: {endpointEdit.StatusCode}). Respuesta: {responseContent}";
                        }
                        return View(model);
                    }

                    resultReserva = JsonSerializer.Deserialize<OperationResult<ReservaDTO>>(responseContent, _jsonSerializerOptions);

                    if (resultReserva != null && resultReserva.Success)
                    {
                        TempData["Success"] = resultReserva.Message ?? "Reserva actualizada exitosamente";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["Error"] = resultReserva?.Message ?? "Error al actualizar la reserva";
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
            OperationResult<ReservaDTO> resultReserva = null;
            try
            {
                using (var httpclient = new HttpClient())
                {
                    httpclient.BaseAddress = new Uri(BaseApiAddress);

                    var endpoint = await httpclient.GetAsync($"Reserva/{id}");

                    if (!endpoint.IsSuccessStatusCode)
                    {
                        TempData["Error"] = $"Error al consumir la API: {endpoint.StatusCode}";
                        return RedirectToAction("Index");
                    }

                    var content = await endpoint.Content.ReadAsStringAsync();
                    resultReserva = JsonSerializer.Deserialize<OperationResult<ReservaDTO>>(content, _jsonSerializerOptions);

                    if (resultReserva != null && resultReserva.Success && resultReserva.Data != null)
                    {
                        TempData["Success"] = resultReserva.Message;
                        return PartialView("_Delete", resultReserva.Data);
                    }
                    else
                    {
                        TempData["Error"] = resultReserva?.Message ?? "Error desconocido al obtener la reserva para eliminar.";
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

                    var endpointRemove = await httpclient.DeleteAsync($"Reserva/{id}");

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
                        return Json(new { success = false, message = $"Error {result?.Message ?? "Error desconocido al confirmar la eliminación"}" });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al consumir la API: {ex.Message}" });
            }
        }

        private async Task<List<ReservaDTO>> GetReservasAsync(bool suppressTempData = false)
        {
            try
            {
                using (var httpclient = new HttpClient())
                {
                    httpclient.BaseAddress = new Uri(BaseApiAddress);
                    var endpoint = await httpclient.GetAsync("Reserva");

                    if (!endpoint.IsSuccessStatusCode)
                    {
                        if (!suppressTempData)
                            TempData["Error"] = $"Error al consumir la API: {endpoint.StatusCode}";
                        return new List<ReservaDTO>();
                    }

                    var responseString = await endpoint.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<OperationResult<List<ReservaDTO>>>(responseString, _jsonSerializerOptions);

                    if (result != null && result.Success)
                    {
                        if (!suppressTempData)
                            TempData["Success"] = result.Message;
                        return result.Data ?? new List<ReservaDTO>();
                    }

                    if (!suppressTempData)
                        TempData["Error"] = result?.Message ?? "Error al obtener las reservas";
                    return new List<ReservaDTO>();
                }
            }
            catch (Exception ex)
            {
                if (!suppressTempData)
                    TempData["Error"] = $"Error al consumir la API: {ex.Message}";
                return new List<ReservaDTO>();
            }
        }
    }
}
