using SGHR.Domain.Validators;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration;
using SGHR.Persistence.Base;
using SGHR.Persistence.Context;
using SGHR.Persistence.Interfaces.Configuracion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SGHR.Persistence.Repositories.Configuration
{
    public sealed class PisoRepository : BaseRepository<Piso>, IPisoRepository
    {
        private readonly SGHRContext _context;
        private readonly ILogger<PisoRepository> _logger;

        public PisoRepository(SGHRContext context, ILogger<PisoRepository> logger) : base(context)
        {
            _context = context;
            _logger = logger;
        }


        public override async Task<OperationResult> SaveEntityAsync(Piso entity)
        {
            var validator = new PisoValidator();
            if (!validator.Validate(entity, out string errorMessage))
            {
                _logger.LogWarning("fallo al guardar piso: {Error}", errorMessage);
                return new OperationResult
                {
                    Success = false,
                    Message = errorMessage
                };
            }
            var result = await base.SaveEntityAsync(entity);

            if (result.Success)
                _logger.LogInformation("Piso guardado correctamente: {Id}", entity.Id); 
            else
                _logger.LogError("Error al guardar piso: {Mensaje}", result.Message); 

            return result;
        }

        public override async Task<OperationResult> UpdateEntityAsync(Piso entity)
        {
            var validator = new PisoValidator();
            if (!validator.Validate(entity, out string errorMessage))
            {
                _logger.LogWarning("Validación fallida al actualizar piso: {Error}", errorMessage);
                return new OperationResult
                {
                    Success = false,
                    Message = errorMessage
                };
            }
            var result = await base.UpdateEntityAsync(entity);

            if (result.Success)
                _logger.LogInformation("Piso actualizado correctamente: {Id}", entity.Id); 
            else
                _logger.LogError("Error al actualizar piso: {Mensaje}", result.Message); 

            return result;
        }

        public override async Task<OperationResult> DeleteEntityAsync(Piso entity)
        {
            try
            {
                var result = await base.DeleteEntityAsync(entity);

                if (result.Success)
                    _logger.LogInformation("Piso eliminado correctamente: {Id}", entity.Id); 
                else
                    _logger.LogWarning("No se encontró el piso a eliminar: {Id}", entity.Id); 

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error eliminando el piso con Id {Id}", entity.Id); 
                return new OperationResult
                {
                    Success = false,
                    Message = $"Error eliminando el piso: {ex.Message}"
                };
            }
        }

        public override async Task<OperationResult> RestoreEntityAsync(Piso entity)
        {
            try
            {
                var result = await base.RestoreEntityAsync(entity);

                if (result.Success)
                    _logger.LogInformation("Piso restaurado correctamente: {Id}", entity.Id); 
                else
                    _logger.LogWarning("No se encontró el piso a restaurar: {Id}", entity.Id); 

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error restaurando el piso con Id {Id}", entity.Id); 
                return new OperationResult
                {
                    Success = false,
                    Message = $"Error restaurando el piso: {ex.Message}"
                };
            }
        }
    }
}
