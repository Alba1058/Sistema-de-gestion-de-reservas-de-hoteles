using Microsoft.AspNetCore.Mvc;
using SGHR.Application.DTOs.Configuration.Piso;
using SGHR.Domain.Base;
using System.Text;
using System.Text.Json;

namespace SGHR.Web.ApiConsumer.Controllers.Configuration
{
    public class PisoApiController : Controller
    {
        private readonly ILogger<PisoApiController> _logger;
        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        private const string BaseApiAddress = "http://localhost:5066/api/";

        public PisoApiController(ILogger<PisoApiController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var pisos = await GetPisosAsync();
            return View(pisos);
        }

        public async Task<IActionResult> _List()
        {
            OperationResult<List<PisoDTO>> result = null;
            try
            {
                using (var httpclient = new HttpClient())
                {
                    httpclient.BaseAddress = new Uri(BaseApiAddress);
                    var endpoint = await httpclient.GetAsync("Piso");
                        
                    if (endpoint.IsSuccessStatusCode)
                    {
                        var responseString = await endpoint.Content.ReadAsStringAsync();
                        result = JsonSerializer.Deserialize<OperationResult<List<PisoDTO>>>(responseString, _jsonSerializerOptions);

                        if (result != null && result.Success)
                        {
                            TempData["Success"] = result.Message;
                            return PartialView("_List", result.Data ?? new List<PisoDTO>());
                        }
                        else
                        {
                            TempData["Error"] = result?.Message ?? "Error al obtener los pisos";
                            return PartialView("_List", new List<PisoDTO>());
                        }
                    }
                    else
                    {
                        TempData["Error"] = $"Error al consumir la API: {endpoint.StatusCode}";
                        return PartialView("_List", new List<PisoDTO>());
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al consumir la API: {ex.Message}";
                return PartialView("_List", new List<PisoDTO>());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            OperationResult<PisoDTO> result = null;
            try
            {
                using (var httpclient = new HttpClient())
                {
                    httpclient.BaseAddress = new Uri(BaseApiAddress);

                    var endpoint = await httpclient.GetAsync($"Piso/{id}");
                    
                    if (!endpoint.IsSuccessStatusCode)
                    {
                        TempData["Error"] = $"Error al consumir la API: {endpoint.StatusCode}";
                        return RedirectToAction("Index");
                    }

                    var content = await endpoint.Content.ReadAsStringAsync();
                    result = JsonSerializer.Deserialize<OperationResult<PisoDTO>>(content, _jsonSerializerOptions);

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
            var model = new CreatePisoDTO();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePisoDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            OperationResult<PisoDTO> resultPiso = null;
            try
            {
                using (var httpclient = new HttpClient())
                {
                    httpclient.BaseAddress = new Uri(BaseApiAddress);

                    var json = JsonSerializer.Serialize(model);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var endpointCreate = await httpclient.PostAsync("Piso", content);
                    
                    if (!endpointCreate.IsSuccessStatusCode)
                    {
                        var errorContent = await endpointCreate.Content.ReadAsStringAsync();
                        var errorResult = JsonSerializer.Deserialize<OperationResult<PisoDTO>>(errorContent, _jsonSerializerOptions);

                        TempData["Error"] = errorResult?.Message ?? $"Error al crear el piso (Código: {endpointCreate.StatusCode})";
                        return View(model);
                    }

                    var responseContent = await endpointCreate.Content.ReadAsStringAsync();
                    resultPiso = JsonSerializer.Deserialize<OperationResult<PisoDTO>>(responseContent, _jsonSerializerOptions);

                    if (resultPiso != null && resultPiso.Success)
                    {
                        TempData["Success"] = resultPiso.Message ?? "Piso creado exitosamente";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["Error"] = resultPiso?.Message ?? "Error al crear el piso";
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
            OperationResult<PisoDTO> resultPiso = null;
            try
            {
                using (var httpclient = new HttpClient())
                {
                    httpclient.BaseAddress = new Uri(BaseApiAddress);

                    var endpoint = await httpclient.GetAsync($"Piso/{id}");
                    
                    if (!endpoint.IsSuccessStatusCode)
                    {
                        TempData["Error"] = $"Error al consumir la API: {endpoint.StatusCode}";
                        return RedirectToAction("Index");
                    }

                    var content = await endpoint.Content.ReadAsStringAsync();
                    resultPiso = JsonSerializer.Deserialize<OperationResult<PisoDTO>>(content, _jsonSerializerOptions);

                    if (resultPiso != null && resultPiso.Success && resultPiso.Data != null)
                    {
                        var piso = resultPiso.Data;
                        var updateDto = new UpdatePisoDTO
                        {
                            Id = piso.Id,
                            Numero = piso.Numero,
                            Descripcion = piso.Descripcion,
                            Estado = piso.Estado
                        };

                        TempData["Success"] = resultPiso.Message;
                        return View(updateDto);
                    }
                    else
                    {
                        TempData["Error"] = resultPiso?.Message ?? "Error desconocido al preparar la edición.";
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
        public async Task<IActionResult> Edit(UpdatePisoDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            OperationResult<PisoDTO> resultPiso = null;
            try
            {
                using (var httpclient = new HttpClient())
                {
                    httpclient.BaseAddress = new Uri(BaseApiAddress);

                    var json = JsonSerializer.Serialize(model);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var endpointEdit = await httpclient.PutAsync("Piso", content);
                    
                    if (!endpointEdit.IsSuccessStatusCode)
                    {
                        var errorContent = await endpointEdit.Content.ReadAsStringAsync();
                        var errorResult = JsonSerializer.Deserialize<OperationResult<PisoDTO>>(errorContent, _jsonSerializerOptions);

                        TempData["Error"] = errorResult?.Message ?? $"Error al actualizar el piso (Código: {endpointEdit.StatusCode})";
                        return View(model);
                    }

                    var responseContent = await endpointEdit.Content.ReadAsStringAsync();
                    resultPiso = JsonSerializer.Deserialize<OperationResult<PisoDTO>>(responseContent, _jsonSerializerOptions);

                    if (resultPiso != null && resultPiso.Success)
                    {
                        TempData["Success"] = resultPiso.Message ?? "Piso actualizado exitosamente";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["Error"] = resultPiso?.Message ?? "Error al actualizar el piso";
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
            OperationResult<PisoDTO> resultPiso = null;
            try
            {
                using (var httpclient = new HttpClient())
                {
                    httpclient.BaseAddress = new Uri(BaseApiAddress);

                    var endpoint = await httpclient.GetAsync($"Piso/{id}");
                    
                    if (!endpoint.IsSuccessStatusCode)
                    {
                        TempData["Error"] = $"Error al consumir la API: {endpoint.StatusCode}";
                        return RedirectToAction("Index");
                    }

                    var content = await endpoint.Content.ReadAsStringAsync();
                    resultPiso = JsonSerializer.Deserialize<OperationResult<PisoDTO>>(content, _jsonSerializerOptions);

                    if (resultPiso != null && resultPiso.Success && resultPiso.Data != null)
                    {
                        TempData["Success"] = resultPiso.Message;
                        return PartialView("_Delete", resultPiso.Data);
                    }
                    else
                    {
                        TempData["Error"] = resultPiso?.Message ?? "Error desconocido al obtener el piso para eliminar.";
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

                    var endpointRemove = await httpclient.DeleteAsync($"Piso/{id}");
                    
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

        private async Task<List<PisoDTO>> GetPisosAsync()
        {
            try
            {
                using (var httpclient = new HttpClient())
                {
                    httpclient.BaseAddress = new Uri(BaseApiAddress);
                    var endpoint = await httpclient.GetAsync("Piso");

                    if (!endpoint.IsSuccessStatusCode)
                    {
                        TempData["Error"] = $"Error al consumir la API: {endpoint.StatusCode}";
                        return new List<PisoDTO>();
                    }

                    var responseString = await endpoint.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<OperationResult<List<PisoDTO>>>(responseString, _jsonSerializerOptions);

                    if (result != null && result.Success)
                    {
                        TempData["Success"] = result.Message;
                        return result.Data ?? new List<PisoDTO>();
                    }

                    TempData["Error"] = result?.Message ?? "Error al obtener los pisos";
                    return new List<PisoDTO>();
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al consumir la API: {ex.Message}";
                return new List<PisoDTO>();
            }
        }
    }
}
