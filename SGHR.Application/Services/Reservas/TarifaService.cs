using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using SGHR.Domain.Base;
using SGHR.Persistence.Interfaces.Reservas;
using SGHR.Application.DTOs.Reservas.Tarifa;
using SGHR.Domain.Entities.Reservas;
using SGHR.Application.Mappers;
using SGHR.Application.Interfaces.Reservas;

namespace SGHR.Application.Services.Reservas
{
    public class TarifaService : ITarifaService
    {
        private readonly ITarifaRepository _tarifaRepository;
        private readonly ILogger<TarifaService> _logger;

        public TarifaService(ITarifaRepository tarifaRepository, ILogger<TarifaService> logger)
        {
            _tarifaRepository = tarifaRepository ?? throw new ArgumentNullException(nameof(tarifaRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OperationResult> GetAll()
        {
            var result = new OperationResult();
            try
            {
                var tarifas = await _tarifaRepository.GetAllAsync();
                result.Data = tarifas.Where(t => !t.IsDeleted).Select(ConfigurationMappers.ToTarifaDto).ToList();
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener tarifas.");
                result.Success = false;
                result.Message = "Ocurrió un error al obtener las tarifas.";
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
                var tarifa = await _tarifaRepository.GetEntityByIdAsync(id);
                if (tarifa == null || tarifa.IsDeleted)
                {
                    result.Success = false;
                    result.Message = "Tarifa no encontrada.";
                    return result;
                }

                result.Data = ConfigurationMappers.ToTarifaDto(tarifa);
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener tarifa Id={Id}", id);
                result.Success = false;
                result.Message = "Ocurrió un error al obtener la tarifa.";
            }

            return result;
        }

        public async Task<OperationResult> Save(CreateTarifaDTO dto)
        {
            var result = new OperationResult();
            if (dto == null)
            {
                result.Success = false;
                result.Message = "Datos de tarifa vacíos.";
                return result;
            }

            try
            {
                var entity = ConfigurationMappers.CreateTarifaEntity(dto);
                var op = await _tarifaRepository.SaveEntityAsync(entity);
                return op;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar tarifa.");
                result.Success = false;
                result.Message = "Ocurrió un error al guardar la tarifa.";
                return result;
            }
        }

        public async Task<OperationResult> Update(UpdateTarifaDTO dto)
        {
            var result = new OperationResult();
            if (dto == null || dto.Id <= 0)
            {
                result.Success = false;
                result.Message = "Id de tarifa inválido.";
                return result;
            }

            try
            {
                var tarifa = await _tarifaRepository.GetEntityByIdAsync(dto.Id);
                if (tarifa == null || tarifa.IsDeleted)
                {
                    result.Success = false;
                    result.Message = "Tarifa no encontrada.";
                    return result;
                }

                ConfigurationMappers.UpdateTarifaFromDto(tarifa, dto);
                var op = await _tarifaRepository.UpdateEntityAsync(tarifa);
                return op;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar tarifa Id={Id}", dto?.Id);
                result.Success = false;
                result.Message = "Ocurrió un error al actualizar la tarifa.";
                return result;
            }
        }

        public async Task<OperationResult> Remove(DeleteTarifaDTO dto)
        {
            var result = new OperationResult();
            if (dto == null || dto.Id <= 0)
            {
                result.Success = false;
                result.Message = "Id de tarifa inválido.";
                return result;
            }

            try
            {
                var tarifa = await _tarifaRepository.GetEntityByIdAsync(dto.Id);
                if (tarifa == null || tarifa.IsDeleted)
                {
                    result.Success = false;
                    result.Message = "Tarifa no encontrada.";
                    return result;
                }

                tarifa.IsDeleted = true;
                tarifa.FechaModificacion = DateTime.UtcNow;
                var op = await _tarifaRepository.UpdateEntityAsync(tarifa);
                return op;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar tarifa Id={Id}", dto?.Id);
                result.Success = false;
                result.Message = "Ocurrió un error al eliminar la tarifa.";
                return result;
            }
        }
    }
}