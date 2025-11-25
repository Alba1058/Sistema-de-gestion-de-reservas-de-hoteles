using Microsoft.AspNetCore.Mvc;
using SGHR.Application.DTOs.Configuration.Categoria;
using SGHR.Domain.Base;
using SGHR.Web.Infrastructure.HttpClients;
using System.Text.Json;

namespace SGHR.Web.ApiConsumer.Controllers.Configuration
{
    public class CategoriaApiController : Controller
    {
        private readonly ILogger<CategoriaApiController> _logger;
        private readonly CategoriaHttpClient _categoriaClient = new CategoriaHttpClient();
        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public CategoriaApiController(ILogger<CategoriaApiController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var categorias = await GetCategoriasAsync();
            return View(categorias);
        }

        public async Task<IActionResult> _List()
        {
            OperationResult<List<CategoriaDTO>> result = null;
            try
            {
                using (var httpclient = new HttpClient())
                {
                    httpclient.BaseAddress = new Uri(BaseApiAddress);
                    var endpoint = await httpclient.GetAsync("Categoria");
                        
                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        result = JsonSerializer.Deserialize<OperationResult<List<CategoriaDTO>>>(responseString, _jsonSerializerOptions);

                        if (result != null && result.Success)
                        {
                            TempData["Success"] = result.Message;
                            return PartialView("_List", result.Data ?? new List<CategoriaDTO>());
                        }
                        else
                        {
                            TempData["Error"] = result?.Message ?? "Error al obtener las categorías";
                            return PartialView("_List", new List<CategoriaDTO>());
                        }
                    }
                    else
                    {
                        TempData["Error"] = $"Error al consumir la API: {response.StatusCode}";
                        return PartialView("_List", new List<CategoriaDTO>());
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al consumir la API: {ex.Message}";
                return PartialView("_List", new List<CategoriaDTO>());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            OperationResult<CategoriaDTO> result = null;
            try
            {
                using (_categoriaClient.client)
                {
                    var response = await _categoriaClient.Details(id);
                    
                    if (!response.IsSuccessStatusCode)
                    {
                        TempData["Error"] = $"Error al consumir la API: {response.StatusCode}";
                        return RedirectToAction("Index");
                    }

                    var content = await response.Content.ReadAsStringAsync();
                    result = JsonSerializer.Deserialize<OperationResult<CategoriaDTO>>(content, _jsonSerializerOptions);

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
            var model = new CreateCategoriaDTO();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCategoriaDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            OperationResult<CategoriaDTO> resultCategoria = null;
            try
            {
                using (_categoriaClient.client)
                {
                    var response = await _categoriaClient.Create(model);
                    
                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        var errorResult = JsonSerializer.Deserialize<OperationResult<CategoriaDTO>>(errorContent, _jsonSerializerOptions);

                        TempData["Error"] = errorResult?.Message ?? $"Error al crear la categoría (Código: {response.StatusCode})";
                        return View(model);
                    }

                    var responseContent = await response.Content.ReadAsStringAsync();
                    resultCategoria = JsonSerializer.Deserialize<OperationResult<CategoriaDTO>>(responseContent, _jsonSerializerOptions);

                    if (resultCategoria != null && resultCategoria.Success)
                    {
                        TempData["Success"] = resultCategoria.Message ?? "Categoría creada exitosamente";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["Error"] = resultCategoria?.Message ?? "Error al crear la categoría";
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
            OperationResult<CategoriaDTO> resultCategoria = null;
            try
            {
                using (_categoriaClient.client)
                {
                    var response = await _categoriaClient.Details(id);
                    
                    if (!response.IsSuccessStatusCode)
                    {
                        TempData["Error"] = $"Error al consumir la API: {response.StatusCode}";
                        return RedirectToAction("Index");
                    }

                    var content = await response.Content.ReadAsStringAsync();
                    resultCategoria = JsonSerializer.Deserialize<OperationResult<CategoriaDTO>>(content, _jsonSerializerOptions);

                    if (resultCategoria != null && resultCategoria.Success && resultCategoria.Data != null)
                    {
                        var categoria = resultCategoria.Data;
                        var updateDto = new UpdateCategoriaDTO
                        {
                            Id = categoria.Id,
                            Nombre = categoria.Nombre,
                            Descripcion = categoria.Descripcion,
                            Estado = categoria.Estado
                        };

                        TempData["Success"] = resultCategoria.Message;
                        return View(updateDto);
                    }
                    else
                    {
                        TempData["Error"] = resultCategoria?.Message ?? "Error desconocido al preparar la edición.";
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
        public async Task<IActionResult> Edit(UpdateCategoriaDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            OperationResult<CategoriaDTO> resultCategoria = null;
            try
            {
                using (_categoriaClient.client)
                {
                    var response = await _categoriaClient.Edit(model);
                    
                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        var errorResult = JsonSerializer.Deserialize<OperationResult<CategoriaDTO>>(errorContent, _jsonSerializerOptions);

                        TempData["Error"] = errorResult?.Message ?? $"Error al actualizar la categoría (Código: {response.StatusCode})";
                        return View(model);
                    }

                    var responseContent = await response.Content.ReadAsStringAsync();
                    resultCategoria = JsonSerializer.Deserialize<OperationResult<CategoriaDTO>>(responseContent, _jsonSerializerOptions);

                    if (resultCategoria != null && resultCategoria.Success)
                    {
                        TempData["Success"] = resultCategoria.Message ?? "Categoría actualizada exitosamente";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["Error"] = resultCategoria?.Message ?? "Error al actualizar la categoría";
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
            OperationResult<CategoriaDTO> resultCategoria = null;
            try
            {
                using (_categoriaClient.client)
                {
                    var response = await _categoriaClient.Details(id);
                    
                    if (!response.IsSuccessStatusCode)
                    {
                        TempData["Error"] = $"Error al consumir la API: {response.StatusCode}";
                        return RedirectToAction("Index");
                    }

                    var content = await response.Content.ReadAsStringAsync();
                    resultCategoria = JsonSerializer.Deserialize<OperationResult<CategoriaDTO>>(content, _jsonSerializerOptions);

                    if (resultCategoria != null && resultCategoria.Success && resultCategoria.Data != null)
                    {
                        TempData["Success"] = resultCategoria.Message;
                        return PartialView("_Delete", resultCategoria.Data);
                    }
                    else
                    {
                        TempData["Error"] = resultCategoria?.Message ?? "Error desconocido al obtener la categoría para eliminar.";
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
                using (_categoriaClient.client)
                {
                    var response = await _categoriaClient.Delete(id);
                    
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

        private async Task<List<CategoriaDTO>> GetCategoriasAsync()
        {
            try
            {
                using (_categoriaClient.client)
                {
                    var response = await _categoriaClient.Index();

                    if (!response.IsSuccessStatusCode)
                    {
                        TempData["Error"] = $"Error al consumir la API: {response.StatusCode}";
                        return new List<CategoriaDTO>();
                    }

                    var responseString = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<OperationResult<List<CategoriaDTO>>>(responseString, _jsonSerializerOptions);

                    if (result != null && result.Success)
                    {
                        TempData["Success"] = result.Message;
                        return result.Data ?? new List<CategoriaDTO>();
                    }

                    TempData["Error"] = result?.Message ?? "Error al obtener las categorías";
                    return new List<CategoriaDTO>();
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al consumir la API: {ex.Message}";
                return new List<CategoriaDTO>();
            }
        }
    }
}
