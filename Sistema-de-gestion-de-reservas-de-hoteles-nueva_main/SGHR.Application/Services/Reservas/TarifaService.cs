using Microsoft.Extensions.Logging;
using SGHR.Application.DTOs.Reservas.Tarifa;
using SGHR.Application.Interfaces.Reservas;
using SGHR.Application.Mappers.Reservas;
using SGHR.Application.Base;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Reservas;
using SGHR.Persistence.Interfaces.Reservas;

namespace SGHR.Application.Services.Reservas
{
    public sealed class TarifaService : ITarifaService
    {
        private readonly ITarifaRepository _tarifaRepository;
        private readonly ILogger<TarifaService> _logger;

        public TarifaService(ITarifaRepository tarifaRepository, ILogger<TarifaService> logger)
        {
            _tarifaRepository = tarifaRepository;
            _logger = logger;
        }

        public async Task<OperationResult<List<TarifaDTO>>> GetAllAsync()
        {
            try
            {
                var tarifas = await _tarifaRepository.GetAllAsync();
                var dtoList = tarifas.Where(t => !t.IsDeleted)
                                     .Select(TarifaMapper.ToTarifaDto)
                                     .ToList();

                return OperationResult<List<TarifaDTO>>.Ok(dtoList, "Tarifas obtenidas correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo tarifas");
                return OperationResult<List<TarifaDTO>>.Fail("Error interno al obtener las tarifas.");
            }
        }

        public async Task<OperationResult<TarifaDTO>> GetByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return OperationResult<TarifaDTO>.Fail("El ID de la tarifa no es válido.");

                var entity = await _tarifaRepository.GetEntityByIdAsync(id);
                if (entity == null)
                    return OperationResult<TarifaDTO>.Fail("Tarifa no encontrada.");

                var dto = TarifaMapper.ToTarifaDto(entity);
                return OperationResult<TarifaDTO>.Ok(dto, "Tarifa obtenida correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo tarifa por Id");
                return OperationResult<TarifaDTO>.Fail("Error interno al obtener la tarifa.");
            }
        }

        public async Task<OperationResult<TarifaDTO>> CreateAsync(CreateTarifaDTO dto)
        {
            try
            {
                // Validaciones
                if (!ValidationHelper.NotNull(dto, "Tarifa", out var msg)) return OperationResult<TarifaDTO>.Fail(msg);
                if (!ValidationHelper.Required(dto.Tipo, "Tipo", out msg)) return OperationResult<TarifaDTO>.Fail(msg);
                if (!ValidationHelper.MaxLength(dto.Tipo, 100, "Tipo", out msg)) return OperationResult<TarifaDTO>.Fail(msg);
                if (!ValidationHelper.MaxLength(dto.Descripcion, 250, "Descripción", out msg)) return OperationResult<TarifaDTO>.Fail(msg);

                if (dto.Monto <= 0)
                    return OperationResult<TarifaDTO>.Fail("El monto debe ser mayor que cero.");

                if (dto.PrecioPorNoche <= 0)
                    return OperationResult<TarifaDTO>.Fail("El precio por noche debe ser mayor que cero.");

                if (dto.FechaFin < dto.FechaInicio)
                    return OperationResult<TarifaDTO>.Fail("La fecha fin no puede ser anterior a la fecha inicio.");

                var entity = TarifaMapper.CreateTarifaEntity(dto, usuario: "sistema");
                var saveResult = await _tarifaRepository.SaveEntityAsync(entity);

                if (!saveResult.Success)
                    return OperationResult<TarifaDTO>.Fail(saveResult.Message);

                var createdDto = TarifaMapper.ToTarifaDto(saveResult.Data!);
                return OperationResult<TarifaDTO>.Ok(createdDto, "Tarifa creada correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear tarifa");
                return OperationResult<TarifaDTO>.Fail("Error interno al crear la tarifa.");
            }
        }

        public async Task<OperationResult<TarifaDTO>> UpdateAsync(UpdateTarifaDTO dto)
        {
            try
            {
                if (dto.Id <= 0) return OperationResult<TarifaDTO>.Fail("El ID no es válido.");
                if (!ValidationHelper.NotNull(dto, "Tarifa", out var msg)) return OperationResult<TarifaDTO>.Fail(msg);

                var entity = await _tarifaRepository.GetEntityByIdAsync(dto.Id);
                if (entity == null) return OperationResult<TarifaDTO>.Fail("Tarifa no encontrada.");

                // Validaciones
                if (!ValidationHelper.Required(dto.Tipo, "Tipo", out msg)) return OperationResult<TarifaDTO>.Fail(msg);
                if (!ValidationHelper.MaxLength(dto.Tipo, 100, "Tipo", out msg)) return OperationResult<TarifaDTO>.Fail(msg);
                if (!ValidationHelper.MaxLength(dto.Descripcion, 250, "Descripción", out msg)) return OperationResult<TarifaDTO>.Fail(msg);

                if (dto.Monto <= 0)
                    return OperationResult<TarifaDTO>.Fail("El monto debe ser mayor que cero.");

                if (dto.PrecioPorNoche <= 0)
                    return OperationResult<TarifaDTO>.Fail("El precio por noche debe ser mayor que cero.");

                if (dto.FechaFin < dto.FechaInicio)
                    return OperationResult<TarifaDTO>.Fail("La fecha fin no puede ser anterior a la fecha inicio.");

                TarifaMapper.UpdateTarifaFromDto(entity, dto, usuario: "sistema");
                var updateOp = await _tarifaRepository.UpdateEntityAsync(entity);

                if (!updateOp.Success)
                    return OperationResult<TarifaDTO>.Fail(updateOp.Message);

                var dtoResult = TarifaMapper.ToTarifaDto(updateOp.Data!);
                return OperationResult<TarifaDTO>.Ok(dtoResult, "Tarifa actualizada correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar tarifa");
                return OperationResult<TarifaDTO>.Fail("Error interno al actualizar la tarifa.");
            }
        }

        public async Task<OperationResult<bool>> RemoveAsync(DeleteTarifaDTO dto)
        {
            try
            {
                if (dto.Id <= 0)
                    return OperationResult<bool>.Fail("El ID no es válido.");

                var entity = await _tarifaRepository.GetEntityByIdAsync(dto.Id);
                if (entity == null)
                    return OperationResult<bool>.Fail("Tarifa no encontrada.");

                var delOp = await _tarifaRepository.DeleteEntityAsync(entity);
                if (!delOp.Success)
                    return OperationResult<bool>.Fail(delOp.Message);

                return OperationResult<bool>.Ok(true, "Tarifa eliminada correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar tarifa");
                return OperationResult<bool>.Fail("Error interno al eliminar la tarifa.");
            }
        }
    }
}
