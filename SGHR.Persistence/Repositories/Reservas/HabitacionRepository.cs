using SGHR.Domain.Validators;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Reservas;
using SGHR.Domain.Enums;
using SGHR.Persistence.Base;
using SGHR.Persistence.Context;
using SGHR.Persistence.Interfaces.Reservas;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SGHR.Persistence.Repositories.Reservas
{
    public sealed class HabitacionRepository : BaseRepository<Habitacion>, IHabitacionRepository
    {
        private readonly SGHRContext _context;
        private readonly ILogger<HabitacionRepository> _logger;

        public HabitacionRepository(SGHRContext context, ILogger<HabitacionRepository> logger) : base(context)
        {
            _context = context;
            _logger = logger;
        }

        // Validacion
        public override async Task<OperationResult> SaveEntityAsync(Habitacion entity)
        {
            var validator = new HabitacionValidator();
            if (!validator.Validate(entity, out string errorMessage))
            {
                _logger.LogWarning("Fallo al guardar habitación: {Error}", errorMessage);
                return new OperationResult { Success = false, Message = errorMessage };
            }

            var result = await base.SaveEntityAsync(entity);

            if (result.Success)
                _logger.LogInformation("Habitación guardada correctamente con Id {Id}", entity.Id);
            else
                _logger.LogError("Error al guardar habitación: {Mensaje}", result.Message);

            return result;
        }

        public override async Task<OperationResult> UpdateEntityAsync(Habitacion entity)
        {
            var validator = new HabitacionValidator();
            if (!validator.Validate(entity, out string errorMessage))
            {
                _logger.LogWarning("Fallo al actualizar habitación: {Error}", errorMessage);
                return new OperationResult { Success = false, Message = errorMessage };
            }

            var result = await base.UpdateEntityAsync(entity);
            if (result.Success)
                _logger.LogInformation("Habitación actualizada correctamente con Id {Id}", entity.Id);
            else
                _logger.LogError("Error al actualizar habitación: {Mensaje}", result.Message);

            return result;
        }

        public async Task<List<Habitacion>> GetHabitacionesDisponiblesAsync()
        {
            try
            {
                var habitaciones = await _context.Habitaciones
                    .Where(h => h.EstadoH == EstadoHabitacion.Disponible && h.Estado && !h.IsDeleted)
                    .ToListAsync();

                _logger.LogInformation("Se obtuvieron {} habitaciones disponibles", habitaciones.Count);
                return habitaciones;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener habitaciones disponibles");
                throw;
            }
        }
    }
}