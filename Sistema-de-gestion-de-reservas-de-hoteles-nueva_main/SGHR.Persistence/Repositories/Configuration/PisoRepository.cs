using Microsoft.Extensions.Logging;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration;
using SGHR.Persistence.Base;
using SGHR.Persistence.Context;
using SGHR.Persistence.Interfaces.Configuration;

namespace SGHR.Persistence.Repositories.Configuration
{
    public sealed class PisoRepository : BaseRepository<Piso>, IPisoRepository
    {
        private readonly ILogger<PisoRepository> _logger;

        public PisoRepository(SGHRContext context, ILogger<PisoRepository> logger)
            : base(context)
        {
            _logger = logger;
        }

        public override async Task<OperationResult<Piso>> SaveEntityAsync(Piso entity)
        {
            var result = await base.SaveEntityAsync(entity);

            if (result.Success)
                _logger.LogInformation(" Piso guardado correctamente: {Id}", entity.Id);
            else
                _logger.LogError(" Error al guardar piso: {Mensaje}", result.Message);

            return result;
        }

        public override async Task<OperationResult<Piso>> UpdateEntityAsync(Piso entity)
        {
            var result = await base.UpdateEntityAsync(entity);

            if (result.Success)
                _logger.LogInformation("Piso actualizado correctamente: {Id}", entity.Id);
            else
                _logger.LogError("Error al actualizar piso: {Mensaje}", result.Message);

            return result;
        }

        public override async Task<OperationResult<bool>> DeleteEntityAsync(Piso entity)
        {
            var result = await base.DeleteEntityAsync(entity);

            if (result.Success)
                _logger.LogInformation("Piso eliminado correctamente: {Id}", entity.Id);
            else
                _logger.LogWarning("No se encontró el piso a eliminar: {Id}", entity.Id);

            return result;
        }

        public override async Task<OperationResult<bool>> RestoreEntityAsync(Piso entity)
        {
            var result = await base.RestoreEntityAsync(entity);

            if (result.Success)
                _logger.LogInformation("Piso restaurado correctamente: {Id}", entity.Id);
            else
                _logger.LogWarning("No se encontró el piso a restaurar: {Id}", entity.Id);

            return result;
        }
    }
}
