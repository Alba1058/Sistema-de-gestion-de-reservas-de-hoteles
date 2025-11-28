using Microsoft.Extensions.Logging;
using SGHR.Domain.Base;

namespace SGHR.Application.Base
{
    public abstract class BaseService
    {
        protected readonly ILogger _logger;

        protected BaseService(ILogger logger)
        {
            _logger = logger;
        }

        protected async Task<OperationResult<TResult>> ExecuteOperationAsync<TResult>(
            Func<Task<OperationResult<TResult>>> operation,
            string defaultErrorMessage)
        {
            try
            {
                return await operation();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ejecutando operación: {Message}", defaultErrorMessage);
                return OperationResult<TResult>.Fail(defaultErrorMessage);
            }
        }

        protected async Task<OperationResult<bool>> ExecuteOperationAsync(
           Func<Task<OperationResult<bool>>> operation,string defaultErrorMessage)
        {
            try
            {
                return await operation();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ejecutando operación: {Message}", defaultErrorMessage);
                return OperationResult<bool>.Fail(defaultErrorMessage);
            }
        }

        protected async Task<OperationResult<List<TDto>>> GetAllEntitiesAsync<TEntity, TDto>(
            Func<Task<List<TEntity>>> repositoryGetAll,
            Func<TEntity, TDto> mapper,
            string entityName) where TEntity : class
        {
            try
            {
                _logger.LogInformation("Iniciando obtención de {EntityName}", entityName);
                
                List<TEntity> entities;
                try
                {
                    entities = await repositoryGetAll();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al obtener {EntityName} del repositorio: {Message}", entityName, ex.Message);
                    return OperationResult<List<TDto>>.Fail($"No se pudieron cargar los {entityName.ToLower()}. Por favor, verifique la conexión con la base de datos.");
                }

                if (entities == null)
                {
                    _logger.LogWarning("El repositorio devolvió null al obtener {EntityName}", entityName);
                    return OperationResult<List<TDto>>.Ok(new List<TDto>(), $"No hay {entityName.ToLower()} disponibles.");
                }

                var totalCount = entities.Count();
                var noEliminadosCount = entities.Count(e => 
                {
                    var isDeletedProperty = e.GetType().GetProperty("IsDeleted");
                    if (isDeletedProperty != null && isDeletedProperty.GetValue(e) is bool isDeleted)
                        return !isDeleted;
                    return true;
                });
                _logger.LogInformation("{EntityName} obtenidos del repositorio: Total={Total}, No eliminados={NoEliminados}", entityName, totalCount, noEliminadosCount);

                if (totalCount == 0)
                {
                    _logger.LogWarning("No hay {EntityName} en la base de datos", entityName);
                    return OperationResult<List<TDto>>.Ok(new List<TDto>(), $"No hay {entityName.ToLower()} disponibles.");
                }

                var dtoList = new List<TDto>();
                var erroresMapeo = 0;
                
                foreach (var entity in entities.Where(e => 
                {
                    var isDeletedProperty = e.GetType().GetProperty("IsDeleted");
                    if (isDeletedProperty != null && isDeletedProperty.GetValue(e) is bool isDeleted)
                        return !isDeleted;
                    return true;
                }))
                {
                    try
                    {
                        var dto = mapper(entity);
                        dtoList.Add(dto);
                        
                        var idProperty = entity.GetType().GetProperty("Id");
                        var nameProperty = entity.GetType().GetProperty("Nombre") ?? entity.GetType().GetProperty("Numero");
                        var idValue = idProperty?.GetValue(entity) ?? 0;
                        var nameValue = nameProperty?.GetValue(entity) ?? "N/A";
                        
                        _logger.LogDebug("{EntityName} mapeado exitosamente: ID={Id}, Nombre={Nombre}", entityName, idValue, nameValue);
                    }
                    catch (Exception ex)
                    {
                        erroresMapeo++;
                        var idProperty = entity.GetType().GetProperty("Id");
                        var nameProperty = entity.GetType().GetProperty("Nombre") ?? entity.GetType().GetProperty("Numero");
                        var idValue = idProperty?.GetValue(entity) ?? 0;
                        var nameValue = nameProperty?.GetValue(entity) ?? "N/A";
                        
                        _logger.LogError(ex, "Error al mapear {EntityName} ID {Id}, Nombre {Nombre}: {Message}", 
                            entityName, idValue, nameValue, ex.Message);
                    }
                }

                _logger.LogInformation("{EntityName} mapeados exitosamente: Total={Total}, No eliminados={NoEliminados}, Mapeados={Mapeados}, Errores={Errores}", 
                    entityName, totalCount, noEliminadosCount, dtoList.Count, erroresMapeo);

                if (dtoList.Count == 0 && noEliminadosCount > 0)
                {
                    _logger.LogError("Todos los {EntityName} no eliminados fallaron al mapearse. Total no eliminados: {Total}, Errores: {Errores}", 
                        entityName, noEliminadosCount, erroresMapeo);
                }

                if (dtoList.Count == 0 && totalCount > 0 && noEliminadosCount == 0)
                {
                    _logger.LogWarning("Todos los {EntityName} están marcados como eliminados. Total: {Total}", entityName, totalCount);
                }

                return OperationResult<List<TDto>>.Ok(dtoList, $"{entityName} obtenidos correctamente. Total: {dtoList.Count}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepción inesperada al obtener {EntityName}: {Message}. StackTrace: {StackTrace}", entityName, ex.Message, ex.StackTrace);
                return OperationResult<List<TDto>>.Fail($"Error al obtener los {entityName.ToLower()}. Por favor, intente más tarde.");
            }
        }

    }
}
