using Microsoft.AspNetCore.Mvc;
using SGHR.Application.DTOs.Reservas.ServicioAdicional;
using SGHR.Domain.Base;
using System.Text;
using System.Text.Json;

namespace SGHR.Web.ApiConsumer.Controllers.Reservas
{
    public class ServicioAdicionalApiController : Controller
    {
        private readonly ILogger<ServicioAdicionalApiController> _logger;
        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        private const string BaseApiAddress = "http://localhost:5066/api/";

        public ServicioAdicionalApiController(ILogger<ServicioAdicionalApiController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var servicios = await GetServiciosAsync();
            return View(servicios);
        }

        public async Task<IActionResult> _List()
        {
            OperationResult<List<ServicioAdicionalDTO>> result = null;
            try
            {
                using (var httpclient = new HttpClient())
                {
                    httpclient.BaseAddress = new Uri(BaseApiAddress);
                    var endpoint = await httpclient.GetAsync("ServicioAdicional");
                        
                    if (endpoint.IsSuccessStatusCode)
                    {
                        var responseString = await endpoint.Content.ReadAsStringAsync();
                        result = JsonSerializer.Deserialize<OperationResult<List<ServicioAdicionalDTO>>>(responseString, _jsonSerializerOptions);

                        if (result != null && result.Success)
                        {
                            TempData["Success"] = result.Message;
                            return PartialView("_List", result.Data ?? new List<ServicioAdicionalDTO>());
                        }
                        else
                        {
                            TempData["Error"] = result?.Message ?? "Error al obtener los servicios";
                            return PartialView("_List", new List<ServicioAdicionalDTO>());
                        }
                    }
                    else
                    {
                        TempData["Error"] = $"Error al consumir la API: {endpoint.StatusCode}";
                        return PartialView("_List", new List<ServicioAdicionalDTO>());
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al consumir la API: {ex.Message}";
                return PartialView("_List", new List<ServicioAdicionalDTO>());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            OperationResult<ServicioAdicionalDTO> result = null;
            try
            {
                using (var httpclient = new HttpClient())
                {
                    httpclient.BaseAddress = new Uri(BaseApiAddress);

                    var endpoint = await httpclient.GetAsync($"ServicioAdicional/{id}");

                    if (!endpoint.IsSuccessStatusCode)
                    {
                        TempData["Error"] = $"Error al consumir la API: {endpoint.StatusCode}";
                        return RedirectToAction("Index");
                    }

                    var content = await endpoint.Content.ReadAsStringAsync();
                    result = JsonSerializer.Deserialize<OperationResult<ServicioAdicionalDTO>>(content, _jsonSerializerOptions);

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
            var model = new CreateServicioAdicionalDTO();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateServicioAdicionalDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            OperationResult<ServicioAdicionalDTO> resultServicio = null;
            try
            {
                using (var httpclient = new HttpClient())
                {
                    httpclient.BaseAddress = new Uri(BaseApiAddress);

                    var json = JsonSerializer.Serialize(model);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var endpointCreate = await httpclient.PostAsync("ServicioAdicional", content);

                    if (!endpointCreate.IsSuccessStatusCode)
                    {
                        var errorContent = await endpointCreate.Content.ReadAsStringAsync();
                        var errorResult = JsonSerializer.Deserialize<OperationResult<ServicioAdicionalDTO>>(errorContent, _jsonSerializerOptions);

                        TempData["Error"] = errorResult?.Message ?? $"Error al crear el servicio (Código: {endpointCreate.StatusCode})";
                        return View(model);
                    }

                    var responseContent = await endpointCreate.Content.ReadAsStringAsync();
                    resultServicio = JsonSerializer.Deserialize<OperationResult<ServicioAdicionalDTO>>(responseContent, _jsonSerializerOptions);

                    if (resultServicio != null && resultServicio.Success)
                    {
                        TempData["Success"] = resultServicio.Message ?? "Servicio creado exitosamente";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["Error"] = resultServicio?.Message ?? "Error al crear el servicio";
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
            OperationResult<ServicioAdicionalDTO> resultServicio = null;
            try
            {
                using (var httpclient = new HttpClient())
                {
                    httpclient.BaseAddress = new Uri(BaseApiAddress);

                    var endpoint = await httpclient.GetAsync($"ServicioAdicional/{id}");

                    if (!endpoint.IsSuccessStatusCode)
                    {
                        TempData["Error"] = $"Error al consumir la API: {endpoint.StatusCode}";
                        return RedirectToAction("Index");
                    }

                    var content = await endpoint.Content.ReadAsStringAsync();
                    resultServicio = JsonSerializer.Deserialize<OperationResult<ServicioAdicionalDTO>>(content, _jsonSerializerOptions);

                    if (resultServicio != null && resultServicio.Success && resultServicio.Data != null)
                    {
                        var servicio = resultServicio.Data;
                        var updateDto = new UpdateServicioAdicionalDTO
                        {
                            Id = servicio.Id,
                            Nombre = servicio.Nombre,
                            Precio = servicio.Precio,
                            Descripcion = servicio.Descripcion,
                            Estado = servicio.Estado
                        };

                        TempData["Success"] = resultServicio.Message;
                        return View(updateDto);
                    }
                    else
                    {
                        TempData["Error"] = resultServicio?.Message ?? "Error desconocido al preparar la edición.";
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
        public async Task<IActionResult> Edit(UpdateServicioAdicionalDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            OperationResult<ServicioAdicionalDTO> resultServicio = null;
            try
            {
                using (var httpclient = new HttpClient())
                {
                    httpclient.BaseAddress = new Uri(BaseApiAddress);

                    var json = JsonSerializer.Serialize(model);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var endpointEdit = await httpclient.PutAsync("ServicioAdicional", content);

                    if (!endpointEdit.IsSuccessStatusCode)
                    {
                        var errorContent = await endpointEdit.Content.ReadAsStringAsync();
                        var errorResult = JsonSerializer.Deserialize<OperationResult<ServicioAdicionalDTO>>(errorContent, _jsonSerializerOptions);

                        TempData["Error"] = errorResult?.Message ?? $"Error al actualizar el servicio (Código: {endpointEdit.StatusCode})";
                        return View(model);
                    }

                    var responseContent = await endpointEdit.Content.ReadAsStringAsync();
                    resultServicio = JsonSerializer.Deserialize<OperationResult<ServicioAdicionalDTO>>(responseContent, _jsonSerializerOptions);

                    if (resultServicio != null && resultServicio.Success)
                    {
                        TempData["Success"] = resultServicio.Message ?? "Servicio actualizado exitosamente";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["Error"] = resultServicio?.Message ?? "Error al actualizar el servicio";
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
            OperationResult<ServicioAdicionalDTO> resultServicio = null;
            try
            {
                using (var httpclient = new HttpClient())
                {
                    httpclient.BaseAddress = new Uri(BaseApiAddress);

                    var endpoint = await httpclient.GetAsync($"ServicioAdicional/{id}");

                    if (!endpoint.IsSuccessStatusCode)
                    {
                        TempData["Error"] = $"Error al consumir la API: {endpoint.StatusCode}";
                        return RedirectToAction("Index");
                    }

                    var content = await endpoint.Content.ReadAsStringAsync();
                    resultServicio = JsonSerializer.Deserialize<OperationResult<ServicioAdicionalDTO>>(content, _jsonSerializerOptions);

                    if (resultServicio != null && resultServicio.Success && resultServicio.Data != null)
                    {
                        TempData["Success"] = resultServicio.Message;
                        return PartialView("_Delete", resultServicio.Data);
                    }
                    else
                    {
                        TempData["Error"] = resultServicio?.Message ?? "Error desconocido al obtener el servicio para eliminar.";
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

                    var endpointRemove = await httpclient.DeleteAsync($"ServicioAdicional/{id}");

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

        private async Task<List<ServicioAdicionalDTO>> GetServiciosAsync()
        {
            try
            {
                using (var httpclient = new HttpClient())
                {
                    httpclient.BaseAddress = new Uri(BaseApiAddress);
                    var endpoint = await httpclient.GetAsync("ServicioAdicional");

                    if (!endpoint.IsSuccessStatusCode)
                    {
                        TempData["Error"] = $"Error al consumir la API: {endpoint.StatusCode}";
                        return new List<ServicioAdicionalDTO>();
                    }

                    var responseString = await endpoint.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<OperationResult<List<ServicioAdicionalDTO>>>(responseString, _jsonSerializerOptions);

                    if (result != null && result.Success)
                    {
                        TempData["Success"] = result.Message;
                        return result.Data ?? new List<ServicioAdicionalDTO>();
                    }

                    TempData["Error"] = result?.Message ?? "Error al obtener los servicios";
                    return new List<ServicioAdicionalDTO>();
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al consumir la API: {ex.Message}";
                return new List<ServicioAdicionalDTO>();
            }
        }
    }
}
