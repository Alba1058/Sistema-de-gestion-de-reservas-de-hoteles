using Microsoft.EntityFrameworkCore;
using SGHR.Application.Validators;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration;
using SGHR.Persistence.Base;
using SGHR.Persistence.Context;
using SGHR.Persistence.Interfaces.Configuracion;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGHR.Persistence.Repositories.Configuration
{
    public sealed class RolUsuarioRepository : BaseRepository<RolUsuario>, IRolUsuarioRepository
    {
        private readonly SGHRContext _context;

        public RolUsuarioRepository(SGHRContext context) : base(context)
        {
            _context = context;
        }

        public override async Task<OperationResult> SaveEntityAsync(RolUsuario entity)
        {
            var validator = new RolUsuarioValidator();
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

        public override async Task<OperationResult> UpdateEntityAsync(RolUsuario entity)
        {
            var validator = new RolUsuarioValidator();
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

        public async Task<List<RolUsuario>> GetRolesActivosAsync()
        {
            return await _context.RolesUsuario
              .Where(r => r.Estado == true)
              .ToListAsync();
        }
    }
}