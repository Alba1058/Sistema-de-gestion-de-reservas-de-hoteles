using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SGHR.Application.DTOs.Configuration.RolUsuario;
using SGHR.Application.Interfaces.Configuration;
using SGHR.Application.Mappers;
using SGHR.Domain.Entities.Configuration;
using SGHR.Persistence.Interfaces.Configuracion;

namespace SGHR.Application.Services.Configuracion
{
    public class RolUsuarioService : IRolUsuarioService
    {
        private readonly IRolUsuarioRepository _rolRepository;
        private readonly ILogger<RolUsuarioService> _logger;

        public RolUsuarioService(IRolUsuarioRepository rolRepository, ILogger<RolUsuarioService> logger)
        {
            _rolRepository = rolRepository ?? throw new ArgumentNullException(nameof(rolRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<RolUsuarioDTO>> GetAllAsync()
        {
            try
            {
                var roles = await _rolRepository.GetAllAsync();
                return roles
                    .Where(r => !r.IsDeleted)
                    .Select(ConfigurationMappers.ToRolUsuarioDto)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener roles de usuario.");
                return Enumerable.Empty<RolUsuarioDTO>();
            }
        }

        public async Task<RolUsuarioDTO?> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Id de rol inválido.");
                return null;
            }

            try
            {
                var rol = await _rolRepository.GetEntityByIdAsync(id);
                if (rol == null || rol.IsDeleted)
                    return null;

                return ConfigurationMappers.ToRolUsuarioDto(rol);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener rol Id={Id}", id);
                return null;
            }
        }

        public async Task<RolUsuarioDTO> CreateAsync(CreateRolUsuarioDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Nombre))
                throw new ArgumentException("El nombre del rol es requerido.");

            try
            {
                var all = await _rolRepository.GetAllAsync();
                if (all.Any(r => !r.IsDeleted && string.Equals(r.Nombre, dto.Nombre, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException("Ya existe un rol activo con ese nombre.");
                }

                var entity = ConfigurationMappers.CreateRolUsuarioEntity(dto);
                await _rolRepository.SaveEntityAsync(entity);
                return ConfigurationMappers.ToRolUsuarioDto(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear rol.");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(UpdateRolUsuarioDTO dto)
        {
            if (dto == null || dto.Id <= 0)
                return false;

            try
            {
                var rol = await _rolRepository.GetEntityByIdAsync(dto.Id);
                if (rol == null || rol.IsDeleted)
                    return false;

                var all = await _rolRepository.GetAllAsync();
                if (!string.Equals(rol.Nombre, dto.Nombre, StringComparison.OrdinalIgnoreCase) &&
                    all.Any(r => r.Id != dto.Id && !r.IsDeleted && string.Equals(r.Nombre, dto.Nombre, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException("Ya existe otro rol activo con ese nombre.");
                }

                rol.Nombre = dto.Nombre ?? rol.Nombre;
                rol.Descripcion = dto.Descripcion ?? rol.Descripcion;
                rol.IsDeleted = !dto.Estado;
                rol.FechaModificacion = DateTime.UtcNow;

                await _rolRepository.UpdateEntityAsync(rol);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar rol Id={Id}", dto.Id);
                return false;
            }
        }

        public async Task<bool> DeleteAsync(DeleteRolUsuarioDTO dto)
        {
            if (dto == null || dto.Id <= 0)
                return false;

            try
            {
                var rol = await _rolRepository.GetEntityByIdAsync(dto.Id);
                if (rol == null || rol.IsDeleted)
                    return false;

                rol.IsDeleted = true;
                rol.FechaModificacion = DateTime.UtcNow;

                await _rolRepository.UpdateEntityAsync(rol);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar rol Id={Id}", dto.Id);
                return false;
            }
        }
    }
}
