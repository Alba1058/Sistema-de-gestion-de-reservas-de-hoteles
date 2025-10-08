using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SGHR.Application.DTOs.Configuration.Piso;
using SGHR.Application.Interfaces.Configuration;
using SGHR.Application.Mappers;
using SGHR.Domain.Entities.Configuration;
using SGHR.Persistence.Interfaces.Configuracion;

namespace SGHR.Application.Services.Configuracion
{
    public class PisoService : IPisoService
    {
        private readonly IPisoRepository _pisoRepository;
        private readonly ILogger<PisoService> _logger;

        public PisoService(IPisoRepository pisoRepository, ILogger<PisoService> logger)
        {
            _pisoRepository = pisoRepository ?? throw new ArgumentNullException(nameof(pisoRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<PisoDTO>> GetAllAsync()
        {
            try
            {
                var pisos = await _pisoRepository.GetAllAsync();
                return pisos
                    .Where(p => !p.IsDeleted)
                    .Select(ConfigurationMappers.ToPisoDto)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los pisos.");
                return Enumerable.Empty<PisoDTO>();
            }
        }

        public async Task<PisoDTO?> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Id de piso inválido.");
                return null;
            }

            try
            {
                var piso = await _pisoRepository.GetEntityByIdAsync(id);
                if (piso == null || piso.IsDeleted)
                    return null;

                return ConfigurationMappers.ToPisoDto(piso);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener piso por Id={Id}", id);
                return null;
            }
        }

        public async Task<PisoDTO> CreateAsync(CreatePisoDTO dto)
        {
            if (dto == null || dto.Numero <= 0)
                throw new ArgumentException("El número de piso es requerido.");

            try
            {
                var all = await _pisoRepository.GetAllAsync();
                if (all.Any(p => !p.IsDeleted && p.Numero == dto.Numero))
                {
                    throw new InvalidOperationException("Ya existe un piso activo con ese número.");
                }

                var entity = ConfigurationMappers.CreatePisoEntity(dto);
                await _pisoRepository.SaveEntityAsync(entity);
                return ConfigurationMappers.ToPisoDto(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear piso.");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(UpdatePisoDTO dto)
        {
            if (dto == null || dto.Id <= 0)
                return false;

            try
            {
                var piso = await _pisoRepository.GetEntityByIdAsync(dto.Id);
                if (piso == null || piso.IsDeleted)
                    return false;

                var all = await _pisoRepository.GetAllAsync();
                if (all.Any(p => p.Id != dto.Id && !p.IsDeleted && p.Numero == dto.Numero))
                {
                    throw new InvalidOperationException("Ya existe otro piso activo con ese número.");
                }

                piso.Numero = dto.Numero;
                piso.Descripcion = dto.Descripcion ?? piso.Descripcion;
                piso.FechaModificacion = DateTime.UtcNow;

                await _pisoRepository.UpdateEntityAsync(piso);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar piso Id={Id}", dto.Id);
                return false;
            }
        }

        public async Task<bool> DeleteAsync(DeletePisoDTO dto)
        {
            if (dto == null || dto.Id <= 0)
                return false;

            try
            {
                var piso = await _pisoRepository.GetEntityByIdAsync(dto.Id);
                if (piso == null || piso.IsDeleted)
                    return false;

                piso.IsDeleted = true;
                piso.FechaModificacion = DateTime.UtcNow;

                await _pisoRepository.UpdateEntityAsync(piso);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar piso Id={Id}", dto.Id);
                return false;
            }
        }
    }
}
