using Microsoft.EntityFrameworkCore;
using SGHR.Application.Validators;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Reservas;
using SGHR.Persistence.Base;
using SGHR.Persistence.Context;
using SGHR.Persistence.Interfaces.Reservas;
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

        public ServiciosAdicionalesRepository(SGHRContext context) : base(context)
        {
            _context = context;
        }

        public override async Task<OperationResult> SaveEntityAsync(ServicioAdicional entity)
        {
            var validator = new ServicioAdicionalValidator();
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
        public override async Task<OperationResult> UpdateEntityAsync(ServicioAdicional entity)
        {
            var validator = new ServicioAdicionalValidator();
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

        public async Task<List<ServicioAdicional>> GetServiciosDisponiblesAsync()
        {
            return await _context.ServiciosAdicionales
                .Where(s => s.Estado == true && s.IsDeleted == false)
                .ToListAsync();
        }
    }
}