using System.Net.Http.Json;
using System.Text.Json;
using SGHR.Domain.Base;

namespace SGHR.Web.Infrastructure.Services.Api.Base
{
    public abstract class BaseApiService<TDto, TCreateDto, TUpdateDto, TDeleteDto>
    {
        protected readonly IHttpClientFactory _httpClientFactory;
        protected readonly ILogger _logger;
        protected readonly JsonSerializerOptions _jsonOptions;

        protected BaseApiService(IHttpClientFactory httpClientFactory, ILogger logger)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        protected abstract string EntityName { get; }
        protected abstract string BaseEndpoint { get; }

        public virtual async Task<OperationResult<List<TDto>>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Obteniendo todos los {EntityName}", EntityName);
                
                using (HttpClient httpClient = _httpClientFactory.CreateClient("SGHRAPI"))
                {
                    var response = await httpClient.GetAsync(BaseEndpoint);

                    if (!response.IsSuccessStatusCode)
                    {
                        var errorMessage = $"Error al obtener {EntityName}: {response.StatusCode}";
                        _logger.LogWarning(errorMessage);
                        return OperationResult<List<TDto>>.Fail(errorMessage);
                    }

                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<OperationResult<List<TDto>>>(content, _jsonOptions);

                    if (result != null && result.Success)
                    {
                        _logger.LogInformation("{EntityName} obtenidos correctamente", EntityName);
                        return result;
                    }

                    return OperationResult<List<TDto>>.Fail(result?.Message ?? $"Error al obtener {EntityName}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener {EntityName}", EntityName);
                return OperationResult<List<TDto>>.Fail($"Error interno al obtener {EntityName}: {ex.Message}");
            }
        }

        public virtual async Task<OperationResult<TDto>> GetByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return OperationResult<TDto>.Fail($"El ID de {EntityName} no es válido.");

                _logger.LogInformation("Obteniendo {EntityName} con ID: {Id}", EntityName, id);
                
                using (HttpClient httpClient = _httpClientFactory.CreateClient("SGHRAPI"))
                {
                    var response = await httpClient.GetAsync($"{BaseEndpoint}/{id}");

                    if (!response.IsSuccessStatusCode)
                    {
                        var errorMessage = $"Error al obtener {EntityName}: {response.StatusCode}";
                        _logger.LogWarning(errorMessage);
                        return OperationResult<TDto>.Fail(errorMessage);
                    }

                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<OperationResult<TDto>>(content, _jsonOptions);

                    if (result != null && result.Success && result.Data != null)
                    {
                        _logger.LogInformation("{EntityName} obtenido correctamente", EntityName);
                        return result;
                    }

                    return OperationResult<TDto>.Fail(result?.Message ?? $"{EntityName} no encontrado.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener {EntityName} por ID", EntityName);
                return OperationResult<TDto>.Fail($"Error interno al obtener {EntityName}: {ex.Message}");
            }
        }

        public virtual async Task<OperationResult<TDto>> CreateAsync(TCreateDto dto)
        {
            try
            {
                if (dto == null)
                    return OperationResult<TDto>.Fail($"El objeto {EntityName} no puede ser nulo.");

                _logger.LogInformation("Creando {EntityName}", EntityName);
                
                using (HttpClient httpClient = _httpClientFactory.CreateClient("SGHRAPI"))
                {
                    var response = await httpClient.PostAsJsonAsync(BaseEndpoint, dto);

                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        var errorResult = JsonSerializer.Deserialize<OperationResult<TDto>>(errorContent, _jsonOptions);
                        var errorMessage = errorResult?.Message ?? $"Error al crear {EntityName} (Código: {response.StatusCode})";
                        _logger.LogError("Error al crear {EntityName}: {Message}", EntityName, errorMessage);
                        return OperationResult<TDto>.Fail(errorMessage);
                    }

                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<OperationResult<TDto>>(content, _jsonOptions);

                    if (result != null && result.Success)
                    {
                        _logger.LogInformation("{EntityName} creado exitosamente", EntityName);
                        return result;
                    }

                    return OperationResult<TDto>.Fail(result?.Message ?? $"Error al crear {EntityName}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear {EntityName}", EntityName);
                return OperationResult<TDto>.Fail($"Error interno al crear {EntityName}: {ex.Message}");
            }
        }

        public virtual async Task<OperationResult<TDto>> UpdateAsync(TUpdateDto dto)
        {
            try
            {
                if (dto == null)
                    return OperationResult<TDto>.Fail($"El objeto {EntityName} no puede ser nulo.");

                _logger.LogInformation("Actualizando {EntityName}", EntityName);
                
                using (HttpClient httpClient = _httpClientFactory.CreateClient("SGHRAPI"))
                {
                    var response = await httpClient.PutAsJsonAsync(BaseEndpoint, dto);

                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        var errorResult = JsonSerializer.Deserialize<OperationResult<TDto>>(errorContent, _jsonOptions);
                        var errorMessage = errorResult?.Message ?? $"Error al actualizar {EntityName} (Código: {response.StatusCode})";
                        _logger.LogError("Error al actualizar {EntityName}: {Message}", EntityName, errorMessage);
                        return OperationResult<TDto>.Fail(errorMessage);
                    }

                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<OperationResult<TDto>>(content, _jsonOptions);

                    if (result != null && result.Success)
                    {
                        _logger.LogInformation("{EntityName} actualizado exitosamente", EntityName);
                        return result;
                    }

                    return OperationResult<TDto>.Fail(result?.Message ?? $"Error al actualizar {EntityName}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar {EntityName}", EntityName);
                return OperationResult<TDto>.Fail($"Error interno al actualizar {EntityName}: {ex.Message}");
            }
        }

        public virtual async Task<OperationResult<bool>> DeleteAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return OperationResult<bool>.Fail($"El ID de {EntityName} no es válido.");

                _logger.LogInformation("Eliminando {EntityName} con ID: {Id}", EntityName, id);
                
                using (HttpClient httpClient = _httpClientFactory.CreateClient("SGHRAPI"))
                {
                    var response = await httpClient.DeleteAsync($"{BaseEndpoint}/{id}");

                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        var errorMessage = $"Error al eliminar {EntityName} (Código: {response.StatusCode}). Detalles: {errorContent}";
                        _logger.LogError(errorMessage);
                        return OperationResult<bool>.Fail(errorMessage);
                    }

                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<OperationResult<bool>>(content, _jsonOptions);

                    if (result != null && result.Success)
                    {
                        _logger.LogInformation("{EntityName} eliminado exitosamente", EntityName);
                        return result;
                    }

                    return OperationResult<bool>.Fail(result?.Message ?? $"Error al eliminar {EntityName}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar {EntityName}", EntityName);
                return OperationResult<bool>.Fail($"Error interno al eliminar {EntityName}: {ex.Message}");
            }
        }
    }
}

