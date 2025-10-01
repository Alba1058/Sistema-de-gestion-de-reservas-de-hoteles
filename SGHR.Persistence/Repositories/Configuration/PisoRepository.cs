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
    public sealed class PisoRepository : BaseRepository<Piso>, IPisoRepository
    {
        private readonly SGHRContext _context;

        public PisoRepository(SGHRContext context) : base(context)
        {
            _context = context;
        }

        public override async Task<OperationResult> SaveEntityAsync(Piso entity)
        {
            var validator = new PisoValidator();
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

        public override async Task<OperationResult> UpdateEntityAsync(Piso entity)
        {
            var validator = new PisoValidator();
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

        public override async Task<OperationResult> DeleteEntityAsync(Piso entity)
        {
            try
            {
                return await base.DeleteEntityAsync(entity);
            }
            catch (Exception ex)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = $"Error eliminando el piso: {ex.Message}"
                };
            }
        }

        public override async Task<OperationResult> RestoreEntityAsync(Piso entity)
        {
            try
            {
                return await base.RestoreEntityAsync(entity);
            }
            catch (Exception ex)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = $"Error restaurando el piso: {ex.Message}"
                };
            }
        }

    }
}
