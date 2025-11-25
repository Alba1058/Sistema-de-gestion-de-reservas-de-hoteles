using Microsoft.AspNetCore.Mvc;
using SGHR.Application.DTOs.Configuration.RolUsuario;
using SGHR.Domain.Base;
using System.Text;
using System.Text.Json;

namespace SGHR.Web.ApiConsumer.Controllers.Configuration
{
    public class RolUsuarioApiController : Controller
    {
        private readonly ILogger<RolUsuarioApiController> _logger;
        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        private const string BaseApiAddress = "http://localhost:5066/api/";

        public RolUsuarioApiController(ILogger<RolUsuarioApiController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var roles = await GetRolesAsync();
            return View(roles);
        }

        public async Task<IActionResult> _List()
        {
            OperationResult<List<RolUsuarioDTO>> result = null;
            try
            {
                using (var httpclient = new HttpClient())
                {
                    httpclient.BaseAddress = new Uri(BaseApiAddress);
                    var endpoint = await httpclient.GetAsync("RolUsuario");
                        
                    if (endpoint.IsSuccessStatusCode)
                    {
                        var responseString = await endpoint.Content.ReadAsStringAsync();
                        result = JsonSerializer.Deserialize<OperationResult<List<RolUsuarioDTO>>>(responseString, _jsonSerializerOptions);

                        if (result != null && result.Success)
                        {
                            TempData["Success"] = result.Message;
                            return PartialView("_List", result.Data ?? new List<RolUsuarioDTO>());
                        }
                        else
                        {
                            TempData["Error"] = result?.Message ?? "Error al obtener los roles";
                            return PartialView("_List", new List<RolUsuarioDTO>());
                        }
                    }
                    else
                    {
                        TempData["Error"] = $"Error al consumir la API: {endpoint.StatusCode}";
                        return PartialView("_List", new List<RolUsuarioDTO>());
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al consumir la API: {ex.Message}";
                return PartialView("_List", new List<RolUsuarioDTO>());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            OperationResult<RolUsuarioDTO> result = null;
            try
            {
                using (var httpclient = new HttpClient())
                {
                    httpclient.BaseAddress = new Uri(BaseApiAddress);

                    var endpoint = await httpclient.GetAsync($"RolUsuario/{id}");
                    
                    if (!endpoint.IsSuccessStatusCode)
                    {
                        TempData["Error"] = $"Error al consumir la API: {endpoint.StatusCode}";
                        return RedirectToAction("Index");
                    }

                    var content = await endpoint.Content.ReadAsStringAsync();
                    result = JsonSerializer.Deserialize<OperationResult<RolUsuarioDTO>>(content, _jsonSerializerOptions);

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
            var model = new CreateRolUsuarioDTO();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateRolUsuarioDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            OperationResult<RolUsuarioDTO> resultRol = null;
            try
            {
                using (var httpclient = new HttpClient())
                {
                    httpclient.BaseAddress = new Uri(BaseApiAddress);

                    var json = JsonSerializer.Serialize(model);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var endpointCreate = await httpclient.PostAsync("RolUsuario", content);
                    
                    if (!endpointCreate.IsSuccessStatusCode)
                    {
                        var errorContent = await endpointCreate.Content.ReadAsStringAsync();
                        var errorResult = JsonSerializer.Deserialize<OperationResult<RolUsuarioDTO>>(errorContent, _jsonSerializerOptions);

                        TempData["Error"] = errorResult?.Message ?? $"Error al crear el rol (Código: {endpointCreate.StatusCode})";
                        return View(model);
                    }

                    var responseContent = await endpointCreate.Content.ReadAsStringAsync();
                    resultRol = JsonSerializer.Deserialize<OperationResult<RolUsuarioDTO>>(responseContent, _jsonSerializerOptions);

                    if (resultRol != null && resultRol.Success)
                    {
                        TempData["Success"] = resultRol.Message ?? "Rol creado exitosamente";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["Error"] = resultRol?.Message ?? "Error al crear el rol";
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
            OperationResult<RolUsuarioDTO> resultRol = null;
            try
            {
                using (var httpclient = new HttpClient())
                {
                    httpclient.BaseAddress = new Uri(BaseApiAddress);

                    var endpoint = await httpclient.GetAsync($"RolUsuario/{id}");
                    
                    if (!endpoint.IsSuccessStatusCode)
                    {
                        TempData["Error"] = $"Error al consumir la API: {endpoint.StatusCode}";
                        return RedirectToAction("Index");
                    }

                    var content = await endpoint.Content.ReadAsStringAsync();
                    resultRol = JsonSerializer.Deserialize<OperationResult<RolUsuarioDTO>>(content, _jsonSerializerOptions);

                    if (resultRol != null && resultRol.Success && resultRol.Data != null)
                    {
                        var rol = resultRol.Data;
                        var updateDto = new UpdateRolUsuarioDTO
                        {
                            Id = rol.Id,
                            Nombre = rol.Nombre,
                            Descripcion = rol.Descripcion,
                            Estado = rol.Estado
                        };

                        TempData["Success"] = resultRol.Message;
                        return View(updateDto);
                    }
                    else
                    {
                        TempData["Error"] = resultRol?.Message ?? "Error desconocido al preparar la edición.";
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
        public async Task<IActionResult> Edit(UpdateRolUsuarioDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            OperationResult<RolUsuarioDTO> resultRol = null;
            try
            {
                using (var httpclient = new HttpClient())
                {
                    httpclient.BaseAddress = new Uri(BaseApiAddress);

                    var json = JsonSerializer.Serialize(model);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var endpointEdit = await httpclient.PutAsync("RolUsuario", content);
                    
                    if (!endpointEdit.IsSuccessStatusCode)
                    {
                        var errorContent = await endpointEdit.Content.ReadAsStringAsync();
                        var errorResult = JsonSerializer.Deserialize<OperationResult<RolUsuarioDTO>>(errorContent, _jsonSerializerOptions);

                        TempData["Error"] = errorResult?.Message ?? $"Error al actualizar el rol (Código: {endpointEdit.StatusCode})";
                        return View(model);
                    }

                    var responseContent = await endpointEdit.Content.ReadAsStringAsync();
                    resultRol = JsonSerializer.Deserialize<OperationResult<RolUsuarioDTO>>(responseContent, _jsonSerializerOptions);

                    if (resultRol != null && resultRol.Success)
                    {
                        TempData["Success"] = resultRol.Message ?? "Rol actualizado exitosamente";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["Error"] = resultRol?.Message ?? "Error al actualizar el rol";
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
            OperationResult<RolUsuarioDTO> resultRol = null;
            try
            {
                using (var httpclient = new HttpClient())
                {
                    httpclient.BaseAddress = new Uri(BaseApiAddress);

                    var endpoint = await httpclient.GetAsync($"RolUsuario/{id}");
                    
                    if (!endpoint.IsSuccessStatusCode)
                    {
                        TempData["Error"] = $"Error al consumir la API: {endpoint.StatusCode}";
                        return RedirectToAction("Index");
                    }

                    var content = await endpoint.Content.ReadAsStringAsync();
                    resultRol = JsonSerializer.Deserialize<OperationResult<RolUsuarioDTO>>(content, _jsonSerializerOptions);

                    if (resultRol != null && resultRol.Success && resultRol.Data != null)
                    {
                        TempData["Success"] = resultRol.Message;
                        return PartialView("_Delete", resultRol.Data);
                    }
                    else
                    {
                        TempData["Error"] = resultRol?.Message ?? "Error desconocido al obtener el rol para eliminar.";
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

                    var endpointRemove = await httpclient.DeleteAsync($"RolUsuario/{id}");
                    
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

        private async Task<List<RolUsuarioDTO>> GetRolesAsync()
        {
            try
            {
                using (var httpclient = new HttpClient())
                {
                    httpclient.BaseAddress = new Uri(BaseApiAddress);
                    var endpoint = await httpclient.GetAsync("RolUsuario");

                    if (!endpoint.IsSuccessStatusCode)
                    {
                        TempData["Error"] = $"Error al consumir la API: {endpoint.StatusCode}";
                        return new List<RolUsuarioDTO>();
                    }

                    var responseString = await endpoint.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<OperationResult<List<RolUsuarioDTO>>>(responseString, _jsonSerializerOptions);

                    if (result != null && result.Success)
                    {
                        TempData["Success"] = result.Message;
                        return result.Data ?? new List<RolUsuarioDTO>();
                    }

                    TempData["Error"] = result?.Message ?? "Error al obtener los roles";
                    return new List<RolUsuarioDTO>();
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al consumir la API: {ex.Message}";
                return new List<RolUsuarioDTO>();
            }
        }
    }
}
