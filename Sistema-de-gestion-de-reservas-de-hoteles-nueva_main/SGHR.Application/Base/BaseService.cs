using Microsoft.Extensions.Logging;
using SGHR.Domain.Base;

namespace SGHR.Application.Base
{
    public abstract class BaseService
    {
        private readonly ILogger _logger;

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

    }
}
