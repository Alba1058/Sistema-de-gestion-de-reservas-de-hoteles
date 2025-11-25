using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Reservas;
using SGHR.Persistence.Base;
using SGHR.Persistence.Context;
using SGHR.Persistence.Interfaces.Reservas;

namespace SGHR.Persistence.Repositories.Reservas
{
    public sealed class ReservaRepository : BaseRepository<Reserva>, IReservaRepository
    {
        private readonly ILogger<ReservaRepository> _logger;

        public ReservaRepository(SGHRContext context, ILogger<ReservaRepository> logger)
            : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override async Task<OperationResult<Reserva>> SaveEntityAsync(Reserva entity)
        {
            var result = new OperationResult<Reserva>();
            try
            {
                // Validar que el cliente existe
                var clienteExiste = await _context.Set<Domain.Entities.Clientes.Cliente>()
                    .AnyAsync(c => c.Id == entity.IdCliente && !c.IsDeleted);
                if (!clienteExiste)
                {
                    _logger.LogWarning("Intento de crear reserva con cliente inexistente: {ClienteId}", entity.IdCliente);
                    return OperationResult<Reserva>.Fail($"El cliente con ID {entity.IdCliente} no existe o está eliminado.");
                }

                // Validar que la habitación existe
                var habitacionExiste = await _context.Set<Habitacion>()
                    .AnyAsync(h => h.Id == entity.IdHabitacion && !h.IsDeleted);
                if (!habitacionExiste)
                {
                    _logger.LogWarning("Intento de crear reserva con habitación inexistente: {HabitacionId}", entity.IdHabitacion);
                    return OperationResult<Reserva>.Fail($"La habitación con ID {entity.IdHabitacion} no existe o está eliminada.");
                }

                await _entities.AddAsync(entity);
                await _context.SaveChangesAsync();

                result.Data = entity;
                result.Success = true;
                result.Message = "Reserva guardada correctamente.";
                _logger.LogInformation("Reserva guardada correctamente con Id {Id}", entity.Id);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Error de base de datos al guardar reserva.");
                var innerMessage = dbEx.InnerException?.Message ?? dbEx.Message;
                
                if (innerMessage.Contains("FOREIGN KEY") || innerMessage.Contains("foreign key"))
                {
                    result.Message = "Error: El cliente o la habitación especificados no existen en la base de datos.";
                }
                else if (innerMessage.Contains("UNIQUE") || innerMessage.Contains("unique"))
                {
                    result.Message = "Error: Ya existe una reserva con estos datos.";
                }
                else
                {
                    result.Message = $"Error al guardar la reserva: {innerMessage}";
                }
                result.Success = false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno al guardar reserva.");
                result.Success = false;
                result.Message = $"Error interno al guardar la reserva: {ex.Message}";
            }
            return result;
        }

        public override async Task<OperationResult<Reserva>> UpdateEntityAsync(Reserva entity)
        {
            try
            {
                var result = await base.UpdateEntityAsync(entity);
                if (result.Success)
                    _logger.LogInformation("Reserva actualizada correctamente con Id {Id}", entity.Id);
                else
                    _logger.LogWarning("Error al actualizar reserva: {Mensaje}", result.Message);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno al actualizar reserva.");
                return OperationResult<Reserva>.Fail("Error interno al actualizar la reserva.");
            }
        }

        public async Task<OperationResult<List<Reserva>>> GetReservasPorFechaAsync(DateTime inicio, DateTime fin)
        {
            try
            {
                var reservas = await _context.Set<Reserva>()
                    .Where(r => !r.IsDeleted &&
                                r.FechaInicio < fin &&
                                r.FechaFin > inicio)
                    .ToListAsync();

                if (!reservas.Any())
                {
                    _logger.LogWarning("No se encontraron reservas entre {Inicio} y {Fin}", inicio, fin);
                    return OperationResult<List<Reserva>>.Fail("No se encontraron reservas en ese rango de fechas.");
                }

                _logger.LogInformation("Se obtuvieron {Count} reservas entre {Inicio} y {Fin}", reservas.Count, inicio, fin);
                return OperationResult<List<Reserva>>.Ok(reservas, "Reservas obtenidas correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener reservas por fecha.");
                return OperationResult<List<Reserva>>.Fail("Error interno al obtener reservas por fecha.");
            }
        }

        public async Task<OperationResult<List<Reserva>>> GetReservasPorClienteAsync(int clienteId)
        {
            try
            {
                var reservas = await _context.Set<Reserva>()
                    .Where(r => !r.IsDeleted && r.IdCliente == clienteId)
                    .ToListAsync();

                if (!reservas.Any())
                {
                    _logger.LogWarning("No se encontraron reservas para el cliente {ClienteId}", clienteId);
                    return OperationResult<List<Reserva>>.Fail("El cliente no tiene reservas registradas.");
                }

                _logger.LogInformation("Se obtuvieron {Count} reservas para el cliente {ClienteId}", reservas.Count, clienteId);
                return OperationResult<List<Reserva>>.Ok(reservas, "Reservas obtenidas correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener reservas por cliente.");
                return OperationResult<List<Reserva>>.Fail("Error interno al obtener reservas por cliente.");
            }
        }

        public async Task<OperationResult<bool>> CancelarReservaAsync(int reservaId)
        {
            try
            {
                var reserva = await _context.Set<Reserva>()
                    .FirstOrDefaultAsync(r => r.Id == reservaId && !r.IsDeleted);

                if (reserva == null)
                {
                    _logger.LogWarning("Intento de cancelar reserva no encontrada: {Id}", reservaId);
                    return OperationResult<bool>.Fail("Reserva no encontrada.");
                }

                reserva.IsDeleted = true;
                reserva.FechaModificacion = DateTime.UtcNow;

                var updateResult = await base.UpdateEntityAsync(reserva);
                if (!updateResult.Success)
                {
                    _logger.LogWarning("Error al cancelar reserva {Id}: {Mensaje}", reservaId, updateResult.Message);
                    return OperationResult<bool>.Fail("Error al cancelar la reserva.");
                }

                _logger.LogInformation("Reserva {Id} cancelada correctamente.", reservaId);
                return OperationResult<bool>.Ok(true, "Reserva cancelada correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno al cancelar reserva {Id}", reservaId);
                return OperationResult<bool>.Fail("Error interno al cancelar la reserva.");
            }
        }
    }
}
