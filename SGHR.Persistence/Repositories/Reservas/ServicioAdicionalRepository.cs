using Microsoft.EntityFrameworkCore;
using SGHR.Domain.Validators;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Reservas;
using SGHR.Persistence.Base;
using SGHR.Persistence.Context;
using SGHR.Persistence.Interfaces.Reservas;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGHR.Persistence.Repositories.Reservas
{
    public sealed class ServiciosAdicionalesRepository : BaseRepository<ServicioAdicional>, IServicioAdicionalRepository
    {
        private readonly SGHRContext _context;
        private readonly ILogger<ServiciosAdicionalesRepository> _logger;

        public ServiciosAdicionalesRepository(SGHRContext context, ILogger<ServiciosAdicionalesRepository> logger) : base(context)
        {
            _context = context;
            _logger = logger;
        }

        public override async Task<OperationResult> SaveEntityAsync(ServicioAdicional entity)
        {
            var validator = new ServicioAdicionalValidator();
            if (!validator.Validate(entity, out string errorMessage))
            {
                _logger.LogWarning("fallo al guardar servicio adicional: {Error}", errorMessage); 
                return new OperationResult { Success = false, Message = errorMessage };
            }

            var result = await base.SaveEntityAsync(entity);
            if (result.Success)
                _logger.LogInformation("Servicio adicional guardado correctamente con Id {Id}", entity.Id); 
            else
                _logger.LogError("Error al guardar servicio adicional: {Mensaje}", result.Message); 

            return result;
        }

        public override async Task<OperationResult> UpdateEntityAsync(ServicioAdicional entity)
        {
            var validator = new ServicioAdicionalValidator();
            if (!validator.Validate(entity, out string errorMessage))
            {
                _logger.LogWarning("fallo al actualizar servicio adicional: {Error}", errorMessage);
                return new OperationResult { Success = false, Message = errorMessage };
            }

            var result = await base.UpdateEntityAsync(entity);
            if (result.Success)
                _logger.LogInformation("Servicio adicional actualizado correctamente con Id {Id}", entity.Id); 
            else
                _logger.LogError("Error al actualizar servicio adicional: {Mensaje}", result.Message); 

            return result;
        }

        public async Task<List<ServicioAdicional>> GetServiciosDisponiblesAsync()
        {
            try
            {
                var servicios = await _context.ServiciosAdicionales
                    .Where(s => s.Estado && !s.IsDeleted)
                    .ToListAsync();

                _logger.LogInformation("Se obtuvieron {Count} servicios adicionales disponibles", servicios.Count); 
                return servicios;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener servicios adicionales disponibles"); 
                throw;
            }
        }
    }
}