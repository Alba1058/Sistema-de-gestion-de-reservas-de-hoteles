using SGHR.Domain.Validators;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Reservas;
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
    public sealed class ReservaRepository : BaseRepository<Reserva>, IReservaRepository
    {
        private readonly SGHRContext _context;
        private readonly ILogger<ReservaRepository> _logger; 

        public ReservaRepository(SGHRContext context, ILogger<ReservaRepository> logger) : base(context)
        {
            _context = context;
            _logger = logger;
        }

        // Validación 
        public override async Task<OperationResult> SaveEntityAsync(Reserva entity)
        {
            var validator = new ReservaValidator();
            if (!validator.Validate(entity, out string errorMessage))
            {
                _logger.LogWarning("Fallo al guardar reserva: {Error}", errorMessage); 
                return new OperationResult { Success = false, Message = errorMessage };
            }

            var result = await base.SaveEntityAsync(entity);
            if (result.Success)
                _logger.LogInformation("Reserva guardada correctamente con Id {Id}", entity.Id); 
            else
                _logger.LogError("Error al guardar reserva: {Mensaje}", result.Message);

            return result;
        }

        public override async Task<OperationResult> UpdateEntityAsync(Reserva entity)
        {
            var validator = new ReservaValidator();
            if (!validator.Validate(entity, out string errorMessage))
            {
                _logger.LogWarning("Fallo al actualizar reserva: {Error}", errorMessage); 
                return new OperationResult { Success = false, Message = errorMessage };
            }

            var result = await base.UpdateEntityAsync(entity);
            if (result.Success)
                _logger.LogInformation("Reserva actualizada correctamente con Id {Id}", entity.Id); 
            else
                _logger.LogError("Error al actualizar reserva: {Mensaje}", result.Message); 

            return result;
        }

        public async Task<List<Reserva>> GetReservasPorFechaAsync(DateTime inicio, DateTime fin)
        {
            try
            {
                var reservas = await _context.Reservas
                    .Where(r => r.FechaInicio >= inicio && r.FechaFin <= fin && !r.IsDeleted)
                    .ToListAsync();

                _logger.LogInformation("Se encontraron {Count} reservas entre {Inicio} y {Fin}", reservas.Count, inicio, fin); 
                return reservas;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener reservas por fecha");
                throw;
            }
        }

        public async Task<List<Reserva>> GetReservasPorClienteAsync(int clienteId)
        {
            try
            {
                var reservas = await _context.Reservas
                    .Where(r => r.IdCliente == clienteId && !r.IsDeleted)
                    .ToListAsync();

                _logger.LogInformation("Se encontraron {Count} reservas para el cliente {ClienteId}", reservas.Count, clienteId); 
                return reservas;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener reservas del cliente {ClienteId}", clienteId); 
                throw;
            }
        }

        public async Task<bool> CancelarReservaAsync(int reservaId)
        {
            try
            {
                var reserva = await _context.Reservas.FindAsync(reservaId);
                if (reserva == null)
                {
                    _logger.LogWarning("Intento de cancelar reserva que no existe: {Id}", reservaId); 
                }

                reserva.EstadoReserva = Domain.Enums.EstadoReserva.Cancelada;
                reserva.FechaModificacion = DateTime.UtcNow;

                _context.Reservas.Update(reserva);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Reserva {Id} cancelada correctamente", reservaId); 
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cancelar la reserva {Id}", reservaId); 
                return false;
            }
        }
    }
}