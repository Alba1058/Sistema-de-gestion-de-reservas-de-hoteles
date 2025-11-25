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
            var operationResult = new OperationResult<Usuario>();

            // Validaciones
            if (entity == null)
                return OperationResult<Usuario>.Fail("El objeto usuario no puede ser nulo.");

            if (string.IsNullOrWhiteSpace(entity.Nombre))
                return OperationResult<Usuario>.Fail("El nombre del usuario no puede estar vacío.");

            if (string.IsNullOrWhiteSpace(entity.Email))
                return OperationResult<Usuario>.Fail("El correo electrónico no puede estar vacío.");

            if (await _context.Usuarios.AnyAsync(u => u.Email == entity.Email && !u.IsDeleted))
                return OperationResult<Usuario>.Fail("Ya existe un usuario con ese correo electrónico.");

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

                    operationResult = OperationResult<Usuario>.Ok(savedEntityWithRol ?? entity, "Usuario guardado correctamente.");
                }
                else
                {
                    _logger.LogWarning("Error al guardar usuario: {Mensaje}", result.Message);
                    operationResult = OperationResult<Usuario>.Fail(result.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno al guardar el usuario");
                operationResult = OperationResult<Usuario>.Fail("Error interno al guardar el usuario.");
            }

            return operationResult;
        }

        public override async Task<OperationResult<Usuario>> UpdateEntityAsync(Usuario entity)
        {
            var operationResult = new OperationResult<Usuario>();

            try
            {
                var existing = await _context.Usuarios.FindAsync(entity.Id);
                if (existing == null)
                    return OperationResult<Usuario>.Fail("Usuario no encontrado.");

                if (await _context.Usuarios.AnyAsync(u => u.Email == entity.Email && u.Id != entity.Id && !u.IsDeleted))
                    return OperationResult<Usuario>.Fail("Ya existe otro usuario con ese correo electrónico.");

                // Asignaciones
                existing.Nombre = entity.Nombre;
                existing.Email = entity.Email;
                existing.RolUsuarioId = entity.RolUsuarioId;
                existing.UsuarioModificacion = entity.UsuarioModificacion;

                existing.FechaModificacion = entity.FechaModificacion;
                existing.IsDeleted = entity.IsDeleted;
                existing.Estado = entity.Estado; 

                _context.Usuarios.Update(existing);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Usuario {Email} actualizado correctamente con ID {Id}", existing.Email, existing.Id);
                operationResult = OperationResult<Usuario>.Ok(existing, "Usuario actualizado correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar usuario");
                operationResult = OperationResult<Usuario>.Fail("Error interno al actualizar el usuario.");
            }

            return operationResult;
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
