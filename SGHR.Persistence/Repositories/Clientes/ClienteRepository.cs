using Microsoft.EntityFrameworkCore;
using SGHR.Application.Validators;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Clientes;
using SGHR.Persistence.Base;
using SGHR.Persistence.Context;
using SGHR.Persistence.Interfaces.Clientes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGHR.Persistence.Repositories.Clientes
{
    public sealed class ClienteRepository : BaseRepository<Cliente>, IClienteRepository
    {
        private readonly SGHRContext _context;

        public ClienteRepository(SGHRContext context) : base(context)
        {
            _context = context;
        }

        // Validación 
        public override async Task<OperationResult> SaveEntityAsync(Cliente entity)
        {
            var validator = new ClienteValidator();
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

        public override async Task<OperationResult> UpdateEntityAsync(Cliente entity)
        {
            var validator = new ClienteValidator();
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
        public async Task<List<Cliente>> GetClientesConReservasAsync()
        {
            return await _context.Clientes
                .Include(c => c.Reservas)
                .Where(c => c.Reservas.Any())
                .ToListAsync();
        }
    }
}