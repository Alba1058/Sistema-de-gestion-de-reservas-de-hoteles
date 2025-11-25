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
            if (entity == null)
                return OperationResult<RolUsuario>.Fail("El rol no puede ser nulo.");

            if (string.IsNullOrWhiteSpace(entity.Nombre))
                return OperationResult<RolUsuario>.Fail("El nombre del rol no puede estar vacío.");

            if (await _context.RolesUsuario.AnyAsync(r => r.Nombre == entity.Nombre && !r.IsDeleted))
                return OperationResult<RolUsuario>.Fail("Ya existe un rol con ese nombre.");

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
                var existing = await _context.RolesUsuario.FindAsync(entity.Id);
                if (existing == null)
                    return OperationResult<RolUsuario>.Fail("Rol no encontrado.");

                if (await _context.RolesUsuario.AnyAsync(r => r.Nombre == entity.Nombre && r.Id != entity.Id && !r.IsDeleted))
                    return OperationResult<RolUsuario>.Fail("Ya existe otro rol con ese nombre.");

                existing.Nombre = entity.Nombre;
                existing.Descripcion = entity.Descripcion;
                existing.Estado = entity.Estado;
                existing.UsuarioModificacion = entity.UsuarioModificacion;
                existing.FechaModificacion = DateTime.Now;

                _context.RolesUsuario.Update(existing);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Rol {NombreRol} actualizado correctamente con ID {Id}", existing.Nombre, existing.Id);

                return OperationResult<RolUsuario>.Ok(existing, "Rol actualizado correctamente.");
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
