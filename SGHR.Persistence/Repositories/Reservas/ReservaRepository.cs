using SGHR.Application.Validators;
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

namespace SGHR.Persistence.Repositories.Reservas
{
    public sealed class ReservaRepository : BaseRepository<Reserva>, IReservaRepository
    {
        private readonly SGHRContext _context;

        public ReservaRepository(SGHRContext context) : base(context)
        {
            _context = context;
        }

        // Validación 
        public override async Task<OperationResult> SaveEntityAsync(Reserva entity)
        {
            var validator = new ReservaValidator();
            if (!validator.Validate(entity, out string errorMessage))
            {
                return new OperationResult
                {
                    Success = false,
                    Message = errorMessage
                };
            }
            return await base.SaveEntityAsync(entity);
        }

        public override async Task<OperationResult> UpdateEntityAsync(Reserva entity)
        {
            var validator = new ReservaValidator();
            if (!validator.Validate(entity, out string errorMessage))
            {
                return new OperationResult
                {
                    Success = false,
                    Message = errorMessage
                };
            }
            return await base.UpdateEntityAsync(entity);
        }

        public async Task<List<Reserva>> GetReservasPorFechaAsync(DateTime inicio, DateTime fin)
        {
            return await _context.Reservas
                .Where(r => r.FechaInicio >= inicio && r.FechaFin <= fin)
                .ToListAsync();
        }

        public async Task<List<Reserva>> GetReservasPorClienteAsync(int clienteId)
        {
            return await _context.Reservas
                .Where(r => r.IdCliente == clienteId)
                .ToListAsync();
        }

        public async Task<bool> CancelarReservaAsync(int reservaId)
        {
            var reserva = await _context.Reservas.FindAsync(reservaId);
            if (reserva == null)
                return false;

            reserva.EstadoReserva = Domain.Enums.EstadoReserva.Cancelada;
            reserva.FechaModificacion = DateTime.UtcNow;
            _context.Reservas.Update(reserva);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}