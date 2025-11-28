using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Usuarios;
using SGHR.Persistence.Base;
using SGHR.Persistence.Context;
using SGHR.Persistence.Interfaces.Usuarios;
using System;
using System.Threading.Tasks;

namespace SGHR.Persistence.Repositories.Usuarios
{
    public sealed class UsuarioRepository : BaseRepository<Usuario>, IUsuarioRepository
    {
        private readonly ILogger<UsuarioRepository> _logger;

        public UsuarioRepository(SGHRContext context, ILogger<UsuarioRepository> logger)
            : base(context)
        {
            _logger = logger;
        }

        public override async Task<OperationResult<Usuario>> SaveEntityAsync(Usuario entity)
        {
            try
            {
                var result = await base.SaveEntityAsync(entity);

                if (result.Success)
                {
                    _logger.LogInformation("Usuario {Email} guardado correctamente con ID {Id}", entity.Email, entity.Id);

                    var savedEntityWithRol = await _context.Usuarios
                        .Include(u => u.RolUsuario)
                        .AsNoTracking() 
                        .FirstOrDefaultAsync(u => u.Id == entity.Id);

                    return OperationResult<Usuario>.Ok(savedEntityWithRol ?? entity, "Usuario guardado correctamente.");
                }
                else
                {
                    _logger.LogWarning("Error al guardar usuario: {Mensaje}", result.Message);
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno al guardar el usuario");
                return OperationResult<Usuario>.Fail("Error interno al guardar el usuario.");
            }
        }

        public override async Task<OperationResult<Usuario>> UpdateEntityAsync(Usuario entity)
        {
            try
            {
                var result = await base.UpdateEntityAsync(entity);

                if (result.Success)
                {
                    _logger.LogInformation("Usuario {Email} actualizado correctamente con ID {Id}", entity.Email, entity.Id);

                    var updatedEntityWithRol = await _context.Usuarios
                        .Include(u => u.RolUsuario)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(u => u.Id == entity.Id);

                    return OperationResult<Usuario>.Ok(updatedEntityWithRol ?? entity, "Usuario actualizado correctamente.");
                }
                else
                {
                    _logger.LogWarning("Error al actualizar usuario: {Mensaje}", result.Message);
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar usuario");
                return OperationResult<Usuario>.Fail("Error interno al actualizar el usuario.");
            }
        }

        public async Task<Usuario?> GetUsuarioByCorreoAsync(string correo)
        {
            try
            {
                var usuario = await _context.Usuarios
                    .Include(u => u.RolUsuario) 
                    .FirstOrDefaultAsync(u => u.Email == correo && !u.IsDeleted);

                if (usuario != null)
                    _logger.LogInformation("Usuario obtenido por correo {Correo}", correo);
                else
                    _logger.LogWarning("No se encontró usuario con el correo {Correo}", correo);

                return usuario;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario por correo {Correo}", correo);
                return null;
            }
        }

        public override async Task<List<Usuario>> GetAllAsync()
        {
            return await _context.Set<Usuario>()
                .Include(u => u.RolUsuario)
                .Where(u => !u.IsDeleted) 
                .ToListAsync();
        }
        public override async Task<Usuario?> GetEntityByIdAsync(int id)
        {
            return await _context.Set<Usuario>()
                .Include(u => u.RolUsuario)
                .FirstOrDefaultAsync(u => u.Id == id);
        }
    }
}
