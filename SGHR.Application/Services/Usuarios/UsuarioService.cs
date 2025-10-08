using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SGHR.Application.DTOs.Usuarios.Usuario;
using SGHR.Application.Interfaces.Usuarios;
using SGHR.Application.Mappers;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Usuarios;
using SGHR.Persistence.Interfaces.Usuarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGHR.Application.Services.Usuarios
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ILogger<UsuarioService> _logger;
        private readonly IConfiguration _configuration;

        public UsuarioService(
            IUsuarioRepository usuarioRepository,
            ILogger<UsuarioService> logger,
            IConfiguration configuration)
        {
            _usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration;
        }

        public async Task<IEnumerable<UsuarioDTO>> GetAllAsync()
        {
            try
            {
                var usuarios = await _usuarioRepository.GetAllAsync();
                var activos = usuarios
                    .Where(u => !u.IsDeleted)
                    .Select(ConfigurationMappers.ToUsuarioDto)
                    .ToList();

                _logger.LogInformation("Usuarios obtenidos correctamente: {Cantidad}", activos.Count);
                return activos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los usuarios.");
                throw;
            }
        }

        public async Task<UsuarioDTO?> GetByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    _logger.LogWarning("Id inválido al intentar obtener usuario.");
                    return null;
                }

                var usuario = await _usuarioRepository.GetEntityByIdAsync(id);

                if (usuario == null || usuario.IsDeleted)
                {
                    _logger.LogWarning("Usuario no encontrado con Id {Id}", id);
                    return null;
                }

                return ConfigurationMappers.ToUsuarioDto(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario por Id {Id}", id);
                throw;
            }
        }

        public async Task<UsuarioDTO> CreateAsync(UsuarioCreateDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Los datos del usuario no pueden ser nulos.");

            try
            {
                // Verificar duplicados
                if (!string.IsNullOrWhiteSpace(dto.Email))
                {
                    var existing = await _usuarioRepository.GetUsuarioByCorreoAsync(dto.Email);
                    if (existing != null && !existing.IsDeleted)
                    {
                        _logger.LogWarning("Intento de crear usuario con correo duplicado: {Correo}", dto.Email);
                        throw new InvalidOperationException("Ya existe un usuario activo con ese correo.");
                    }
                }

                var entity = ConfigurationMappers.CreateUsuarioEntity(dto);
                var result = await _usuarioRepository.SaveEntityAsync(entity);

                if (!result.Success)
                {
                    _logger.LogError("Error al crear usuario: {Mensaje}", result.Message);
                    throw new Exception(result.Message);
                }

                _logger.LogInformation("Usuario creado correctamente: {Id}", entity.Id);
                return ConfigurationMappers.ToUsuarioDto(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el usuario.");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(UsuarioUpdateDTO dto)
        {
            if (dto == null || dto.Id <= 0)
                throw new ArgumentException("Id de usuario inválido.", nameof(dto));

            try
            {
                var usuario = await _usuarioRepository.GetEntityByIdAsync(dto.Id);
                if (usuario == null || usuario.IsDeleted)
                {
                    _logger.LogWarning("No se puede actualizar. Usuario no encontrado con Id {Id}", dto.Id);
                    return false;
                }

                if (!string.IsNullOrWhiteSpace(dto.Email) &&
                    !string.Equals(usuario.Email, dto.Email, StringComparison.OrdinalIgnoreCase))
                {
                    var existing = await _usuarioRepository.GetUsuarioByCorreoAsync(dto.Email);
                    if (existing != null && existing.Id != dto.Id && !existing.IsDeleted)
                    {
                        _logger.LogWarning("Intento de actualizar con correo duplicado: {Correo}", dto.Email);
                        return false;
                    }
                }

                ConfigurationMappers.UpdateUsuarioFromDto(usuario, dto);
                var result = await _usuarioRepository.UpdateEntityAsync(usuario);

                if (!result.Success)
                {
                    _logger.LogError("Error al actualizar usuario: {Mensaje}", result.Message);
                    return false;
                }

                _logger.LogInformation("Usuario actualizado correctamente: {Id}", dto.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el usuario.");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(UsuarioDeleteDTO dto)
        {
            if (dto == null || dto.Id <= 0)
                throw new ArgumentException("Id de usuario inválido.", nameof(dto));

            try
            {
                var usuario = await _usuarioRepository.GetEntityByIdAsync(dto.Id);
                if (usuario == null || usuario.IsDeleted)
                {
                    _logger.LogWarning("No se puede eliminar. Usuario no encontrado con Id {Id}", dto.Id);
                    return false;
                }

                usuario.IsDeleted = true;
                usuario.FechaModificacion = DateTime.UtcNow;

                var result = await _usuarioRepository.UpdateEntityAsync(usuario);

                if (!result.Success)
                {
                    _logger.LogError("Error al eliminar usuario: {Mensaje}", result.Message);
                    return false;
                }

                _logger.LogInformation("Usuario eliminado correctamente: {Id}", dto.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el usuario.");
                throw;
            }
        }

        public async Task<UsuarioDTO?> GetByCorreoAsync(string correo)
        {
            try
            {
                var usuario = await _usuarioRepository.GetUsuarioByCorreoAsync(correo);

                if (usuario == null)
                {
                    _logger.LogWarning("Usuario no encontrado con correo: {Correo}", correo);
                    return null;
                }

                return ConfigurationMappers.ToUsuarioDto(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario por correo: {Correo}", correo);
                throw;
            }
        }
    }
}
