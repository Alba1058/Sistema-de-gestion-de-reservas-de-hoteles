using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Reservas;
using SGHR.Persistence.Base;
using SGHR.Persistence.Context;
using SGHR.Persistence.Interfaces.Reservas;

namespace SGHR.Persistence.Repositories.Reservas
{
    public sealed class ServicioAdicionalRepository : BaseRepository<ServicioAdicional>, IServicioAdicionalRepository
    {
        private readonly ILogger<ServicioAdicionalRepository> _logger;

        public ServicioAdicionalRepository(SGHRContext context, ILogger<ServicioAdicionalRepository> logger)
            : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override async Task<OperationResult<ServicioAdicional>> SaveEntityAsync(ServicioAdicional entity)
        {
            try
            {
                var result = await base.SaveEntityAsync(entity);

                if (result.Success)
                    _logger.LogInformation("Servicio adicional guardado correctamente: {Nombre} (Id: {Id})", entity.Nombre, entity.Id);
                else
                    _logger.LogWarning("Error al guardar servicio adicional: {Mensaje}", result.Message);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno al guardar el servicio adicional.");
                return OperationResult<ServicioAdicional>.Fail("Error interno al guardar el servicio adicional.");
            }
        }

        public override async Task<OperationResult<ServicioAdicional>> UpdateEntityAsync(ServicioAdicional entity)
        {
            try
            {
                var result = await base.UpdateEntityAsync(entity);

                if (result.Success)
                    _logger.LogInformation("Servicio adicional actualizado correctamente: {Nombre} (Id: {Id})", entity.Nombre, entity.Id);
                else
                    _logger.LogWarning("Error al actualizar servicio adicional: {Mensaje}", result.Message);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno al actualizar el servicio adicional.");
                return OperationResult<ServicioAdicional>.Fail("Error interno al actualizar el servicio adicional.");
            }
        }

        public async Task<List<ServicioAdicional>> GetServiciosDisponiblesAsync()
        {
            try
            {
                var servicios = await _context.Set<ServicioAdicional>()
                    .Where(s => !s.IsDeleted && s.Estado)
                    .ToListAsync();

                _logger.LogInformation("Se obtuvieron {Count} servicios adicionales disponibles.", servicios.Count);
                return servicios;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener servicios adicionales disponibles.");
                return new List<ServicioAdicional>();
            }
        }
    }
}
