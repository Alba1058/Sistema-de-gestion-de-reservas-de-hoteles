using Microsoft.AspNetCore.Mvc;
using SGHR.Application.DTOs.Usuarios.Usuario;
using SGHR.Domain.Base;
using SGHR.Web.Infrastructure.HttpClients;
using System.Text.Json;

namespace SGHR.Web.ApiConsumer.Controllers.Usuarios
{
    public class UsuarioApiController : Controller
    {
        private readonly ILogger<UsuarioApiController> _logger;
        private readonly UsuarioHttpClient _usuarioClient = new UsuarioHttpClient();
        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public UsuarioApiController(ILogger<UsuarioApiController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var usuarios = await GetUsuariosAsync();
            return View(usuarios);
        }

        public async Task<IActionResult> _List()
        {
            OperationResult<List<UsuarioDTO>> result = null;
            try
            {
                using (_usuarioClient.client)
                {
                    var response = await _usuarioClient.Index();
                        
                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        result = JsonSerializer.Deserialize<OperationResult<List<UsuarioDTO>>>(responseString, _jsonSerializerOptions);

                        if (result != null && result.Success)
                        {
                            TempData["Success"] = result.Message;
                            return PartialView("_List", result.Data ?? new List<UsuarioDTO>());
                        }
                        else
                        {
                            TempData["Error"] = result?.Message ?? "Error al obtener los usuarios";
                            return PartialView("_List", new List<UsuarioDTO>());
                        }
                    }
                    else
                    {
                        TempData["Error"] = $"Error al consumir la API: {response.StatusCode}";
                        return PartialView("_List", new List<UsuarioDTO>());
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al consumir la API: {ex.Message}";
                return PartialView("_List", new List<UsuarioDTO>());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            OperationResult<UsuarioDTO> result = null;
            try
            {
                using (_usuarioClient.client)
                {
                    var response = await _usuarioClient.Details(id);
                    
                    if (!response.IsSuccessStatusCode)
                    {
                        TempData["Error"] = $"Error al consumir la API: {response.StatusCode}";
                        return RedirectToAction("Index");
                    }

                    var content = await response.Content.ReadAsStringAsync();
                    result = JsonSerializer.Deserialize<OperationResult<UsuarioDTO>>(content, _jsonSerializerOptions);

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
            var model = new UsuarioCreateDTO();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsuarioCreateDTO model)
        {

            _logger.LogInformation($"UsuarioApiController.Create POST: Iniciando creación de usuario. ModelState.IsValid: {ModelState.IsValid}");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning($"UsuarioApiController.Create POST: Errores de validación del modelo. Model: {JsonSerializer.Serialize(model)}");
                return View(model);
            }

            OperationResult<UsuarioDTO> resultUsuario = null;
            try
            {
                using (_usuarioClient.client)
                {
                    var json = JsonSerializer.Serialize(model);
                    _logger.LogInformation($"UsuarioApiController.Create POST: JSON enviado a la API: {json}");
                    
                    var response = await _usuarioClient.Create(model);
                    var responseContent = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation($"UsuarioApiController.Create POST: Respuesta de la API (StatusCode: {response.StatusCode}). Contenido: {responseContent}");
                    
                    if (!response.IsSuccessStatusCode)
                    {
                        try
                        {
                            var errorResult = JsonSerializer.Deserialize<OperationResult<UsuarioDTO>>(responseContent, _jsonSerializerOptions);
                            TempData["Error"] = errorResult?.Message ?? $"Error al crear el usuario (Código: {response.StatusCode}). Respuesta: {responseContent}";
                        }
                        catch (JsonException jex)
                        {
                            TempData["Error"] = $"Error al crear el usuario (Código: {response.StatusCode}). No se pudo deserializar la respuesta de error. Detalles: {responseContent}. Excepción: {jex.Message}";
                        }
                        catch
                        {
                            TempData["Error"] = $"Error al crear el usuario (Código: {response.StatusCode}). Respuesta: {responseContent}";
                        }
                        return View(model);
                    }

                    resultUsuario = JsonSerializer.Deserialize<OperationResult<UsuarioDTO>>(responseContent, _jsonSerializerOptions);

                    if (resultUsuario != null && resultUsuario.Success)
                    {
                        TempData["Success"] = resultUsuario.Message ?? "Usuario creado exitosamente";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["Error"] = resultUsuario?.Message ?? $"Error al crear el usuario. Respuesta: {responseContent}";
                        return View(model);
                    }
                }
            }
            catch (Exception ex)
            {
                var stackTrace = ex.StackTrace != null && ex.StackTrace.Length > 500 ? ex.StackTrace.Substring(0, 500) : ex.StackTrace ?? "";
                _logger.LogError(ex, $"UsuarioApiController.Create POST: Excepción al consumir la API. Mensaje: {ex.Message}. StackTrace: {stackTrace}");
                TempData["Error"] = $"Error al consumir la API: {ex.Message}. StackTrace: {stackTrace}";
                return View(model);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            OperationResult<UsuarioDTO> resultUsuario = null;
            try
            {
                using (_usuarioClient.client)
                {
                    var response = await _usuarioClient.Details(id);
                    
                    if (!response.IsSuccessStatusCode)
                    {
                        TempData["Error"] = $"Error al consumir la API: {response.StatusCode}";
                        return RedirectToAction("Index");
                    }

                    var content = await response.Content.ReadAsStringAsync();
                    resultUsuario = JsonSerializer.Deserialize<OperationResult<UsuarioDTO>>(content, _jsonSerializerOptions);

                    if (resultUsuario != null && resultUsuario.Success && resultUsuario.Data != null)
                    {
                        var usuario = resultUsuario.Data;
                        var updateDto = new UsuarioUpdateDTO
                        {
                            Id = usuario.Id,
                            Nombre = usuario.Nombre,
                            Email = usuario.Email,
                            Activo = usuario.Activo,
                            RolUsuarioId = usuario.RolUsuarioId
                        };

                        TempData["Success"] = resultUsuario.Message;
                        return View(updateDto);
                    }
                    else
                    {
                        TempData["Error"] = resultUsuario?.Message ?? "Error desconocido al preparar la edición.";
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
        public async Task<IActionResult> Edit(UsuarioUpdateDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            OperationResult<UsuarioDTO> resultUsuario = null;
            try
            {
                using (_usuarioClient.client)
                {
                    var response = await _usuarioClient.Edit(model);
                    
                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        var errorResult = JsonSerializer.Deserialize<OperationResult<UsuarioDTO>>(errorContent, _jsonSerializerOptions);

                        TempData["Error"] = errorResult?.Message ?? $"Error al actualizar el usuario (Código: {response.StatusCode})";
                        return View(model);
                    }

                    var responseContent = await response.Content.ReadAsStringAsync();
                    resultUsuario = JsonSerializer.Deserialize<OperationResult<UsuarioDTO>>(responseContent, _jsonSerializerOptions);

                    if (resultUsuario != null && resultUsuario.Success)
                    {
                        TempData["Success"] = resultUsuario.Message ?? "Usuario actualizado exitosamente";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["Error"] = resultUsuario?.Message ?? "Error al actualizar el usuario";
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
            OperationResult<UsuarioDTO> resultUsuario = null;
            try
            {
                using (_usuarioClient.client)
                {
                    var response = await _usuarioClient.Details(id);
                    
                    if (!response.IsSuccessStatusCode)
                    {
                        TempData["Error"] = $"Error al consumir la API: {response.StatusCode}";
                        return RedirectToAction("Index");
                    }

                    var content = await response.Content.ReadAsStringAsync();
                    resultUsuario = JsonSerializer.Deserialize<OperationResult<UsuarioDTO>>(content, _jsonSerializerOptions);

                    if (resultUsuario != null && resultUsuario.Success && resultUsuario.Data != null)
                    {
                        TempData["Success"] = resultUsuario.Message;
                        return PartialView("_Delete", resultUsuario.Data);
                    }
                    else
                    {
                        TempData["Error"] = resultUsuario?.Message ?? "Error desconocido al obtener el usuario para eliminar.";
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
                using (_usuarioClient.client)
                {
                    var response = await _usuarioClient.Delete(id);
                    
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
                        return Json(new { success = false, message = $"Error {result?.Message ?? "Error desconocido al confirmar la eliminación"}" });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al consumir la API: {ex.Message}" });
            }
        }

        private async Task<List<UsuarioDTO>> GetUsuariosAsync()
        {
            try
            {
                using (_usuarioClient.client)
                {
                    var response = await _usuarioClient.Index();

                    if (!response.IsSuccessStatusCode)
                    {
                        TempData["Error"] = $"Error al consumir la API: {response.StatusCode}";
                        return new List<UsuarioDTO>();
                    }

                    var responseString = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<OperationResult<List<UsuarioDTO>>>(responseString, _jsonSerializerOptions);

                    if (result != null && result.Success)
                    {
                        TempData["Success"] = result.Message;
                        return result.Data ?? new List<UsuarioDTO>();
                    }

                    TempData["Error"] = result?.Message ?? "Error al obtener los usuarios";
                    return new List<UsuarioDTO>();
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al consumir la API: {ex.Message}";
                return new List<UsuarioDTO>();
            }
        }
    }
}
