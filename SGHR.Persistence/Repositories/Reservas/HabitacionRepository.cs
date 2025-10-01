using SGHR.Application.Validators;
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

namespace SGHR.Persistence.Repositories.Reservas
{
    public sealed class HabitacionRepository : BaseRepository<Habitacion>, IHabitacionRepository
    {
        private readonly SGHRContext _context;

        public HabitacionRepository(SGHRContext context) : base(context)
        {
            _context = context;
        }

        // Validacion
        public override async Task<OperationResult> SaveEntityAsync(Habitacion entity)
        {
            var validator = new HabitacionValidator();
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

        public override async Task<OperationResult> UpdateEntityAsync(Habitacion entity)
        {
            var validator = new HabitacionValidator();
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

        public async Task<List<Habitacion>> GetHabitacionesDisponiblesAsync()
        {
            return await _context.Habitaciones
                .Where(h => h.EstadoH == EstadoHabitacion.Disponible && h.Estado == true && h.IsDeleted == false)
                .ToListAsync();
        }
    }
}