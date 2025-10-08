using Microsoft.EntityFrameworkCore;
using SGHR.Domain.Validators;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration;
using SGHR.Persistence.Base;
using SGHR.Persistence.Context;
using SGHR.Persistence.Interfaces.Configuracion;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<RolUsuarioRepository> _logger; 

        public RolUsuarioRepository(SGHRContext context, ILogger<RolUsuarioRepository> logger) : base(context)
        {
            _context = context;
            _logger = logger;
        }

        public override async Task<OperationResult> SaveEntityAsync(RolUsuario entity)
        {
            var validator = new RolUsuarioValidator();
            if (!validator.Validate(entity, out string errorMessage))
            {
                _logger.LogWarning("fallo al guardar rol: {Error}", errorMessage);
                return new OperationResult
                {
                    Success = false,
                    Message = errorMessage
                };
            }
            var result = await base.SaveEntityAsync(entity);

            if (result.Success)
                _logger.LogInformation("Rol guardado correctamente: {Id}", entity.Id); 
            else
                _logger.LogError("Error al guardar rol: {Mensaje}", result.Message); 

            return result;
        }

        public override async Task<OperationResult> UpdateEntityAsync(RolUsuario entity)
        {
            var validator = new RolUsuarioValidator();
            if (!validator.Validate(entity, out string errorMessage))
            {

                _logger.LogWarning("fallo al actualizar rol: {Error}", errorMessage);
                return new OperationResult
                {
                    Success = false,
                    Message = errorMessage
                };
            }
            var result = await base.UpdateEntityAsync(entity);

            if (result.Success)
                _logger.LogInformation("Rol actualizado correctamente: {Id}", entity.Id); 
            else
                _logger.LogError("Error al actualizar rol: {Mensaje}", result.Message); 

            return result;
        }

        public async Task<List<RolUsuario>> GetRolesActivosAsync()
        {
            try
            {
                var roles = await _context.RolesUsuario
                    .Where(r => r.Estado == true && !r.IsDeleted)
                    .ToListAsync();

                _logger.LogInformation("Se obtuvieron {} roles activos", roles.Count); 
                return roles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los roles activos"); 
                throw;
            }
        }
    }
}