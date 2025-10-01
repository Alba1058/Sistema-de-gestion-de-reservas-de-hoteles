using Microsoft.EntityFrameworkCore;
using SGHR.Application.Validators;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Usuarios;
using SGHR.Persistence.Base;
using SGHR.Persistence.Context;
using SGHR.Persistence.Interfaces.Usuarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGHR.Persistence.Repositories.Usuarios
{
    public sealed class UsuarioRepository : BaseRepository<Usuario>, IUsuarioRepository
    {
        private readonly SGHRContext _context;

        public UsuarioRepository(SGHRContext context) : base(context)
        {
            _context = context;
        }

        public async override Task<OperationResult> SaveEntityAsync(Usuario entity)
        {
            OperationResult operationResult = new OperationResult();

            // validaciones de datos usando UsuarioValidator
            var validator = new UsuarioValidator();
            if (!validator.Validate(entity, out string errorMessage))
            {
                operationResult.Success = false;
                operationResult.Message = errorMessage;
                return operationResult;
            }

            return await base.SaveEntityAsync(entity);
        }

        public override Task<OperationResult> UpdateEntityAsync(Usuario entity)
        {
            var validator = new UsuarioValidator();
            if (!validator.Validate(entity, out string errorMessage))
            {
                OperationResult operationResult = new OperationResult
                {
                    Success = false,
                    Message = errorMessage
                };
                return Task.FromResult(operationResult);
            }

            return base.UpdateEntityAsync(entity);
        }

        public async Task<Usuario?> GetUsuarioByCorreoAsync(string correo)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == correo);
        }
    }
}
