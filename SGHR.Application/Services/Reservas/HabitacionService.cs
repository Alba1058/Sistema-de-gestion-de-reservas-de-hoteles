using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using SGHR.Application.DTOs.Reservas.Habitacion;
using SGHR.Application.Interfaces.Reservas;
using SGHR.Application.Mappers;
using SGHR.Domain.Entities.Reservas;
using SGHR.Persistence.Interfaces.Reservas;

namespace SGHR.Application.Services.Reservas
{
    public class HabitacionService : IHabitacionService
    {
        private readonly IHabitacionRepository _habitacionRepository;
        private readonly ILogger<HabitacionService> _logger;
        private readonly IConfiguration _configuration;

        public HabitacionService(
            IHabitacionRepository habitacionRepository,
            ILogger<HabitacionService> logger,
            IConfiguration configuration)
        {
            _habitacionRepository = habitacionRepository ?? throw new ArgumentNullException(nameof(habitacionRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration;
        }

        public async Task<IEnumerable<HabitacionDTO>> GetAllAsync()
        {
            try
            {
                var habitaciones = await _habitacionRepository.GetAllAsync();
                return habitaciones
                    .Where(h => !h.IsDeleted)
                    .Select(ConfigurationMappers.ToHabitacionDto)
                    .OrderByDescending(h => h.Id)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo las habitaciones.");
                return Enumerable.Empty<HabitacionDTO>();
            }
        }

        public async Task<HabitacionDTO?> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Id inválido en GetByIdAsync: {Id}", id);
                return null;
            }

            try
            {
                var habitacion = await _habitacionRepository.GetEntityByIdAsync(id);
                if (habitacion == null || habitacion.IsDeleted)
                    return null;

                return ConfigurationMappers.ToHabitacionDto(habitacion);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo habitación Id={Id}", id);
                return null;
            }
        }

        public async Task<IEnumerable<HabitacionDTO>> GetDisponiblesAsync()
        {
            try
            {
                var habitaciones = await _habitacionRepository.GetAllAsync();
                return habitaciones
                    .Where(h => !h.IsDeleted && h.EstadoH == SGHR.Domain.Enums.EstadoHabitacion.Disponible)
                    .Select(ConfigurationMappers.ToHabitacionDto)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo habitaciones disponibles.");
                return Enumerable.Empty<HabitacionDTO>();
            }
        }

        public async Task<HabitacionDTO> CreateAsync(CreateHabitacionDTO dto)
        {
            if (dto == null)
                throw new ArgumentException("Datos de habitación inválidos.");

            try
            {
                var entity = ConfigurationMappers.CreateHabitacionEntity(dto);
                await _habitacionRepository.SaveEntityAsync(entity);
                return ConfigurationMappers.ToHabitacionDto(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear habitación.");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(UpdateHabitacionDTO dto)
        {
            if (dto == null || dto.Id <= 0)
                return false;

            try
            {
                var entity = await _habitacionRepository.GetEntityByIdAsync(dto.Id);
                if (entity == null || entity.IsDeleted)
                    return false;

                ConfigurationMappers.UpdateHabitacionFromDto(entity, dto);
                entity.FechaModificacion = DateTime.UtcNow;

                await _habitacionRepository.UpdateEntityAsync(entity);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar habitación Id={Id}", dto.Id);
                return false;
            }
        }

        public async Task<bool> DeleteAsync(DeleteHabitacionDTO dto)
        {
            if (dto == null || dto.Id <= 0)
                return false;

            try
            {
                var entity = await _habitacionRepository.GetEntityByIdAsync(dto.Id);
                if (entity == null || entity.IsDeleted)
                    return false;

                entity.IsDeleted = true;
                entity.FechaModificacion = DateTime.UtcNow;

                await _habitacionRepository.UpdateEntityAsync(entity);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar habitación Id={Id}", dto.Id);
                return false;
            }
        }
    }
}
