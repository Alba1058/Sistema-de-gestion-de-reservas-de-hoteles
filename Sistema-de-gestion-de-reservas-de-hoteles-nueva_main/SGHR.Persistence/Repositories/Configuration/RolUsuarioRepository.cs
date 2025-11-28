using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration;
using SGHR.Persistence.Base;
using SGHR.Persistence.Context;
using SGHR.Persistence.Interfaces.Configuration;

namespace SGHR.Persistence.Repositories.Configuration
{
    public sealed class RolUsuarioRepository : BaseRepository<RolUsuario>, IRolUsuarioRepository
    {
        private readonly ILogger<RolUsuarioRepository> _logger;

        public RolUsuarioRepository(SGHRContext context, ILogger<RolUsuarioRepository> logger)
            : base(context)
        {
            _logger = logger;
        }

        public override async Task<OperationResult<RolUsuario>> SaveEntityAsync(RolUsuario entity)
        {
            try
            {
                var result = await base.SaveEntityAsync(entity);

                if (result.Success)
                    _logger.LogInformation("Rol {NombreRol} guardado correctamente con ID {Id}", entity.Nombre, entity.Id);
                else
                    _logger.LogWarning("Error al guardar rol: {Mensaje}", result.Message);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno al guardar rol de usuario.");
                return OperationResult<RolUsuario>.Fail("Error interno al guardar rol de usuario.");
            }
        }

        public override async Task<OperationResult<RolUsuario>> UpdateEntityAsync(RolUsuario entity)
        {
            try
            {
                var result = await base.UpdateEntityAsync(entity);

                if (result.Success)
                    _logger.LogInformation("Rol {NombreRol} actualizado correctamente con ID {Id}", entity.Nombre, entity.Id);
                else
                    _logger.LogWarning("Error al actualizar rol: {Mensaje}", result.Message);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno al actualizar rol de usuario.");
                return OperationResult<RolUsuario>.Fail("Error interno al actualizar rol de usuario.");
            }
        }

        public async Task<List<RolUsuario>> GetRolesActivosAsync()
        {
            try
            {
                var roles = await _context.RolesUsuario
                    .Where(r => !r.IsDeleted && r.Estado)
                    .ToListAsync();

                _logger.LogInformation("Roles activos recuperados correctamente ({Count})", roles.Count);
                return roles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener roles activos.");
                return new List<RolUsuario>();
            }
        }
    }
}
