using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Reservas;
using SGHR.Domain.Enums;
using SGHR.Persistence.Base;
using SGHR.Persistence.Context;
using SGHR.Persistence.Interfaces.Reservas;

namespace SGHR.Persistence.Repositories.Reservas
{
    public sealed class HabitacionRepository : BaseRepository<Habitacion>, IHabitacionRepository
    {
        private readonly ILogger<HabitacionRepository> _logger;

        public HabitacionRepository(SGHRContext context, ILogger<HabitacionRepository> logger)
            : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override async Task<OperationResult<Habitacion>> SaveEntityAsync(Habitacion entity)
        {
            try
            {
                var result = await base.SaveEntityAsync(entity);

                if (result.Success)
                    _logger.LogInformation("Habitación guardada correctamente: {Numero} (Id: {Id})", entity.Numero, entity.Id);
                else
                    _logger.LogWarning("Error al guardar habitación: {Mensaje}", result.Message);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno al guardar habitación.");
                return OperationResult<Habitacion>.Fail("Error interno al guardar la habitación.");
            }
        }

        public override async Task<OperationResult<Habitacion>> UpdateEntityAsync(Habitacion entity)
        {
            try
            {
                var result = await base.UpdateEntityAsync(entity);

                if (result.Success)
                    _logger.LogInformation("Habitación actualizada correctamente: {Numero} (Id: {Id})", entity.Numero, entity.Id);
                else
                    _logger.LogWarning("Error al actualizar habitación: {Mensaje}", result.Message);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno al actualizar habitación.");
                return OperationResult<Habitacion>.Fail("Error interno al actualizar la habitación.");
            }
        }

        public async Task<List<Habitacion>> GetHabitacionesDisponiblesAsync()
        {
            try
            {
                var habitaciones = await _context.Habitaciones
                    .Where(h => h.EstadoH == EstadoHabitacion.Disponible && h.Estado && !h.IsDeleted)
                    .ToListAsync();

                _logger.LogInformation("Se obtuvieron {Count} habitaciones disponibles", habitaciones.Count);
                return habitaciones;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener habitaciones disponibles.");
                return new List<Habitacion>();
            }
        }
    }
}
