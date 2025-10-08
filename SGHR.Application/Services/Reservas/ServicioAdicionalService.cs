using Microsoft.Extensions.Logging;
using SGHR.Application.DTOs.Reservas.ServicioAdicional;
using SGHR.Application.Interfaces.Reservas;
using SGHR.Application.Mappers;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Reservas;
using SGHR.Persistence.Interfaces.Reservas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGHR.Application.Services.Reservas
{
    public class ServicioAdicionalService : IServicioAdicionalService
    {
        private readonly IServicioAdicionalRepository _servicioAdicionalRepository;
        private readonly ILogger<ServicioAdicionalService> _logger;

        public ServicioAdicionalService(IServicioAdicionalRepository servicioAdicionalRepository, ILogger<ServicioAdicionalService> logger)
        {
            _servicioAdicionalRepository = servicioAdicionalRepository ?? throw new ArgumentNullException(nameof(servicioAdicionalRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OperationResult> GetAll()
        {
            var result = new OperationResult();
            try
            {
                var servicios = await _servicioAdicionalRepository.GetAllAsync();
                result.Data = servicios
                    .Where(s => !s.IsDeleted)
                    .Select(ConfigurationMappers.ToServicioAdicionalDto)
                    .OrderByDescending(s => s.Id)
                    .ToList();

                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener servicios adicionales.");
                result.Success = false;
                result.Message = "Ocurrió un error al obtener los servicios adicionales.";
            }
            return result;
        }

        public async Task<OperationResult> GetById(int id)
        {
            var result = new OperationResult();
            if (id <= 0)
            {
                result.Success = false;
                result.Message = "Id inválido.";
                return result;
            }

            try
            {
                var servicio = await _servicioAdicionalRepository.GetEntityByIdAsync(id);
                if (servicio == null || servicio.IsDeleted)
                {
                    result.Success = false;
                    result.Message = "Servicio adicional no encontrado.";
                    return result;
                }

                result.Data = ConfigurationMappers.ToServicioAdicionalDto(servicio);
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener servicio adicional Id={Id}", id);
                result.Success = false;
                result.Message = "Ocurrió un error al obtener el servicio adicional.";
            }
            return result;
        }

        public async Task<OperationResult> Save(CreateServicioAdicionalDTO dto)
        {
            var result = new OperationResult();
            if (dto == null || string.IsNullOrWhiteSpace(dto.Nombre))
            {
                result.Success = false;
                result.Message = "Datos de servicio adicionales inválidos.";
                return result;
            }

            try
            {
                var all = await _servicioAdicionalRepository.GetAllAsync();
                var duplicate = all.FirstOrDefault(s => !s.IsDeleted && s.Nombre.Equals(dto.Nombre, StringComparison.OrdinalIgnoreCase));
                if (duplicate != null)
                {
                    result.Success = false;
                    result.Message = "Ya existe un servicio adicional activo con ese nombre.";
                    return result;
                }

                var entity = ConfigurationMappers.CreateServicioAdicionalEntity(dto);
                var op = await _servicioAdicionalRepository.SaveEntityAsync(entity);
                if (!op.Success)
                {
                    result.Success = false;
                    result.Message = op.Message ?? "Error al guardar el servicio adicional.";
                    return result;
                }

                result.Success = true;
                result.Message = "Servicio adicional guardado correctamente.";
                result.Data = ConfigurationMappers.ToServicioAdicionalDto(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar servicio adicional.");
                result.Success = false;
                result.Message = "Ocurrió un error al guardar el servicio adicional.";
            }
            return result;
        }

        public async Task<OperationResult> Update(UpdateServicioAdicionalDTO dto)
        {
            var result = new OperationResult();
            if (dto == null || dto.Id <= 0)
            {
                result.Success = false;
                result.Message = "Id inválido.";
                return result;
            }

            try
            {
                var entity = await _servicioAdicionalRepository.GetEntityByIdAsync(dto.Id);
                if (entity == null || entity.IsDeleted)
                {
                    result.Success = false;
                    result.Message = "Servicio adicional no encontrado.";
                    return result;
                }

                if (!string.IsNullOrWhiteSpace(dto.Nombre) &&
                    !dto.Nombre.Equals(entity.Nombre, StringComparison.OrdinalIgnoreCase))
                {
                    var all = await _servicioAdicionalRepository.GetAllAsync();
                    var duplicate = all.FirstOrDefault(s => s.Id != dto.Id && !s.IsDeleted && s.Nombre.Equals(dto.Nombre, StringComparison.OrdinalIgnoreCase));
                    if (duplicate != null)
                    {
                        result.Success = false;
                        result.Message = "Ya existe otro servicio adicional activo con ese nombre.";
                        return result;
                    }
                }

                ConfigurationMappers.UpdateServicioAdicionalFromDto(entity, dto);
                var op = await _servicioAdicionalRepository.UpdateEntityAsync(entity);

                if (!op.Success)
                {
                    result.Success = false;
                    result.Message = op.Message ?? "Error al actualizar el servicio adicional.";
                    return result;
                }

                result.Success = true;
                result.Message = "Servicio adicional actualizado correctamente.";
                result.Data = ConfigurationMappers.ToServicioAdicionalDto(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar servicio adicional Id={Id}", dto?.Id);
                result.Success = false;
                result.Message = "Ocurrió un error al actualizar el servicio adicional.";
            }
            return result;
        }

        public async Task<OperationResult> Remove(DeleteServicioAdicionalDTO dto)
        {
            var result = new OperationResult();
            if (dto == null || dto.Id <= 0)
            {
                result.Success = false;
                result.Message = "Id inválido.";
                return result;
            }

            try
            {
                var entity = await _servicioAdicionalRepository.GetEntityByIdAsync(dto.Id);
                if (entity == null || entity.IsDeleted)
                {
                    result.Success = false;
                    result.Message = "Servicio adicional no encontrado.";
                    return result;
                }

                entity.IsDeleted = true;
                entity.FechaModificacion = DateTime.UtcNow;

                var op = await _servicioAdicionalRepository.UpdateEntityAsync(entity);
                if (!op.Success)
                {
                    result.Success = false;
                    result.Message = op.Message ?? "Error al eliminar el servicio adicional.";
                    return result;
                }

                result.Success = true;
                result.Message = "Servicio adicional eliminado correctamente.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar servicio adicional Id={Id}", dto?.Id);
                result.Success = false;
                result.Message = "Ocurrió un error al eliminar el servicio adicional.";
            }
            return result;
        }

        public async Task<OperationResult> GetDisponiblesAsync()
        {
            var result = new OperationResult();
            try
            {
                var servicios = await _servicioAdicionalRepository.GetAllAsync();
                result.Data = servicios
                    .Where(s => !s.IsDeleted)
                    .Select(ConfigurationMappers.ToServicioAdicionalDto)
                    .ToList();

                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener servicios adicionales disponibles.");
                result.Success = false;
                result.Message = "Ocurrió un error al obtener los servicios disponibles.";
            }
            return result;
        }
    }
}