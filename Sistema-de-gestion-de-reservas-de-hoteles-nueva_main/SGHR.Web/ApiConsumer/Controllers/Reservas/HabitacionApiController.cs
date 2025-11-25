using Microsoft.AspNetCore.Mvc;
using SGHR.Application.DTOs.Reservas.Habitacion;
using SGHR.Domain.Base;
using System.Text;
using System.Text.Json;

namespace SGHR.Web.ApiConsumer.Controllers.Reservas
{
    public class HabitacionApiController : Controller
    {
        private readonly ILogger<HabitacionApiController> _logger;
        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        private const string BaseApiAddress = "http://localhost:5066/api/";

        public HabitacionApiController(ILogger<HabitacionApiController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var habitaciones = await GetHabitacionesAsync();
            return View(habitaciones);
        }

        public async Task<IActionResult> _List()
        {
            OperationResult<List<HabitacionDTO>> result = null; 
            try
            {
                using (var httpclient = new HttpClient()) 
                {
                    httpclient.BaseAddress = new Uri(BaseApiAddress); 
                    var endpoint = await httpclient.GetAsync("Habitacion");

                    if (endpoint.IsSuccessStatusCode)
                    {
                        var responseString = await endpoint.Content.ReadAsStringAsync();
                        result = JsonSerializer.Deserialize<OperationResult<List<HabitacionDTO>>>(responseString, _jsonSerializerOptions);

                        if (result != null && result.Success)
                        {
                            TempData["Success"] = result.Message;
                            return PartialView("_List", result.Data ?? new List<HabitacionDTO>());
                        }
                        else
                        {
                            TempData["Error"] = result?.Message ?? "Error al obtener las habitaciones";
                            return PartialView("_List", new List<HabitacionDTO>());
                        }
                    }
                    else
                    {
                        TempData["Error"] = $"Error al consumir la API: {endpoint.StatusCode}";
                        return PartialView("_List", new List<HabitacionDTO>());
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al consumir la API: {ex.Message}";
                return PartialView("_List", new List<HabitacionDTO>());
            }
        }
        public async Task<IActionResult> Details(int id)
        {
            OperationResult<HabitacionDTO> result = null; 
            try
            {
                using (var httpclient = new HttpClient())
                {
                    httpclient.BaseAddress = new Uri(BaseApiAddress);

                    var endpoint = await httpclient.GetAsync($"Habitacion/{id}");

                    if (!endpoint.IsSuccessStatusCode)
                    {
                        TempData["Error"] = $"Error al consumir la API: {endpoint.StatusCode}";
                        return RedirectToAction("Index");
                    }

                    var content = await endpoint.Content.ReadAsStringAsync();
                    result = JsonSerializer.Deserialize<OperationResult<HabitacionDTO>>(content, _jsonSerializerOptions);

                    if (result != null && result.Success && result.Data != null)
                    {
                        TempData["Success"] = result.Message;
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
            var model = new CreateHabitacionDTO();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateHabitacionDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            OperationResult<HabitacionDTO> resultHabitacion = null; 
            try
            {
                using (var httpclient = new HttpClient())
                {
                    httpclient.BaseAddress = new Uri(BaseApiAddress);

                    var json = JsonSerializer.Serialize(model);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var endpointCreate = await httpclient.PostAsync("Habitacion", content);

                    if (!endpointCreate.IsSuccessStatusCode)
                    {
                        var errorContent = await endpointCreate.Content.ReadAsStringAsync();
                        var errorResult = JsonSerializer.Deserialize<OperationResult<HabitacionDTO>>(errorContent, _jsonSerializerOptions);

                        TempData["Error"] = errorResult?.Message ?? $"Error al crear la habitación (Código: {endpointCreate.StatusCode})";
                        return View(model);
                    }

                    var responseContent = await endpointCreate.Content.ReadAsStringAsync();
                    resultHabitacion = JsonSerializer.Deserialize<OperationResult<HabitacionDTO>>(responseContent, _jsonSerializerOptions);

                    if (resultHabitacion != null && resultHabitacion.Success)
                    {
                        TempData["Success"] = resultHabitacion.Message ?? "Habitación creada exitosamente";
                        return RedirectToAction("Index"); 
                    }
                    else
                    {
                        TempData["Error"] = resultHabitacion?.Message ?? "Error al crear la habitación";
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
            OperationResult<HabitacionDTO> resultHabitacion = null;
            try
            {
                using (var httpclient = new HttpClient())
                {
                    httpclient.BaseAddress = new Uri(BaseApiAddress);

                    var endpoint = await httpclient.GetAsync($"Habitacion/{id}");

                    if (!endpoint.IsSuccessStatusCode)
                    {
                        TempData["Error"] = $"Error al consumir la API: {endpoint.StatusCode}";
                        return RedirectToAction("Index");
                    }

                    var content = await endpoint.Content.ReadAsStringAsync();
                    resultHabitacion = JsonSerializer.Deserialize<OperationResult<HabitacionDTO>>(content, _jsonSerializerOptions);

                    if (resultHabitacion != null && resultHabitacion.Success && resultHabitacion.Data != null)
                    {
                        var habitacion = resultHabitacion.Data;
                        var updateDto = new UpdateHabitacionDTO
                        {
                            Id = habitacion.Id,
                            Numero = habitacion.Numero,
                            IdCategoria = habitacion.IdCategoria,
                            IdPiso = habitacion.IdPiso,
                            EstadoHabitacion = habitacion.EstadoHabitacion,
                            PrecioBase = habitacion.PrecioBase,
                            Descripcion = habitacion.Descripcion,
                            Estado = habitacion.Estado
                        };

                        TempData["Success"] = resultHabitacion.Message;
                        return View(updateDto);
                    }
                    else
                    {
                        TempData["Error"] = resultHabitacion?.Message ?? "Error desconocido al preparar la edición.";
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
        public async Task<IActionResult> Edit(UpdateHabitacionDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            OperationResult<HabitacionDTO> resultHabitacion = null; 
            try
            {
                using (var httpclient = new HttpClient())
                {
                    httpclient.BaseAddress = new Uri(BaseApiAddress);

                    var json = JsonSerializer.Serialize(model);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var endpointEdit = await httpclient.PutAsync("Habitacion", content);

                    if (!endpointEdit.IsSuccessStatusCode)
                    {
                        var errorContent = await endpointEdit.Content.ReadAsStringAsync();
                        var errorResult = JsonSerializer.Deserialize<OperationResult<HabitacionDTO>>(errorContent, _jsonSerializerOptions);

                        TempData["Error"] = errorResult?.Message ?? $"Error al actualizar la habitación (Código: {endpointEdit.StatusCode})";
                        return View(model);
                    }

                    var responseContent = await endpointEdit.Content.ReadAsStringAsync();
                    resultHabitacion = JsonSerializer.Deserialize<OperationResult<HabitacionDTO>>(responseContent, _jsonSerializerOptions);

                    if (resultHabitacion != null && resultHabitacion.Success)
                    {
                        TempData["Success"] = resultHabitacion.Message ?? "Habitación actualizada exitosamente";
                        return RedirectToAction("Index"); 
                    }
                    else
                    {
                        TempData["Error"] = resultHabitacion?.Message ?? "Error al actualizar la habitación";
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
            OperationResult<HabitacionDTO> resultHabitacion = null; 
            try
            {
                using (var httpclient = new HttpClient())
                {
                    httpclient.BaseAddress = new Uri(BaseApiAddress);

                    var endpoint = await httpclient.GetAsync($"Habitacion/{id}");

                    if (!endpoint.IsSuccessStatusCode)
                    {
                        TempData["Error"] = $"Error al consumir la API: {endpoint.StatusCode}";
                        return RedirectToAction("Index");
                    }

                    var content = await endpoint.Content.ReadAsStringAsync();
                    resultHabitacion = JsonSerializer.Deserialize<OperationResult<HabitacionDTO>>(content, _jsonSerializerOptions);

                    if (resultHabitacion != null && resultHabitacion.Success && resultHabitacion.Data != null)
                    {
                        TempData["Success"] = resultHabitacion.Message;
                        return PartialView("_Delete", resultHabitacion.Data);
                    }
                    else
                    {
                        TempData["Error"] = resultHabitacion?.Message ?? "Error desconocido al obtener la habitación para eliminar.";
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

                    var endpointRemove = await httpclient.DeleteAsync($"Habitacion/{id}");

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

        private async Task<List<HabitacionDTO>> GetHabitacionesAsync()
        {
            try
            {
                using (var httpclient = new HttpClient())
                {
                    httpclient.BaseAddress = new Uri(BaseApiAddress);
                    var endpoint = await httpclient.GetAsync("Habitacion");

                    if (!endpoint.IsSuccessStatusCode)
                    {
                        TempData["Error"] = $"Error al consumir la API: {endpoint.StatusCode}";
                        return new List<HabitacionDTO>();
                    }

                    var responseString = await endpoint.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<OperationResult<List<HabitacionDTO>>>(responseString, _jsonSerializerOptions);

                    if (result != null && result.Success)
                    {
                        TempData["Success"] = result.Message;
                        return result.Data ?? new List<HabitacionDTO>();
                    }

                    TempData["Error"] = result?.Message ?? "Error al obtener las habitaciones";
                    return new List<HabitacionDTO>();
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al consumir la API: {ex.Message}";
                return new List<HabitacionDTO>();
            }
        }
    }
}