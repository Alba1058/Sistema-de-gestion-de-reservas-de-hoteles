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
    public sealed class TarifaService : BaseService, ITarifaService
    {
        private readonly ITarifaRepository _tarifaRepository;

        public TarifaService(ITarifaRepository tarifaRepository, ILogger<TarifaService> logger)
            : base(logger)
        {
            _tarifaRepository = tarifaRepository;
        }

        public async Task<OperationResult<List<TarifaDTO>>> GetAllAsync()
        {
            return await GetAllEntitiesAsync<Tarifa, TarifaDTO>(
                _tarifaRepository.GetAllAsync,
                TarifaMapper.ToTarifaDto,
                "Tarifas");
        }

        public async Task<OperationResult<TarifaDTO>> GetByIdAsync(int id)
        {
            return await ExecuteOperationAsync<TarifaDTO>(async () =>
            {
                if (!ValidationHelper.IsValidId(id, "Tarifa", out var msg))
                    return OperationResult<TarifaDTO>.Fail(msg);

                var entity = await _tarifaRepository.GetEntityByIdAsync(id);
                if (!EntityValidationHelper.ValidateEntityExists(entity, "Tarifa", out msg))
                    return OperationResult<TarifaDTO>.Fail(msg);

                var dto = TarifaMapper.ToTarifaDto(entity);
                return OperationResult<TarifaDTO>.Ok(dto, "Tarifa obtenida correctamente.");
            }, "Error interno al obtener la tarifa.");
        }

        public async Task<OperationResult<TarifaDTO>> CreateAsync(CreateTarifaDTO dto)
        {
            return await ExecuteOperationAsync<TarifaDTO>(async () =>
            {
                if (!ValidationHelper.NotNull(dto, "Tarifa", out var msg))
                    return OperationResult<TarifaDTO>.Fail(msg);
                if (!ValidationHelper.Required(dto.Tipo, "Tipo", out msg))
                    return OperationResult<TarifaDTO>.Fail(msg);
                if (!ValidationHelper.MaxLength(dto.Tipo, 100, "Tipo", out msg))
                    return OperationResult<TarifaDTO>.Fail(msg);
                if (!ValidationHelper.MaxLength(dto.Descripcion, 250, "Descripción", out msg))
                    return OperationResult<TarifaDTO>.Fail(msg);

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
            }, "Error interno al crear la tarifa.");
        }

        public async Task<OperationResult<TarifaDTO>> UpdateAsync(UpdateTarifaDTO dto)
        {
            return await ExecuteOperationAsync<TarifaDTO>(async () =>
            {
                if (!ValidationHelper.IsValidId(dto.Id, "Tarifa", out var msg))
                    return OperationResult<TarifaDTO>.Fail(msg);
                if (!ValidationHelper.NotNull(dto, "Tarifa", out msg))
                    return OperationResult<TarifaDTO>.Fail(msg);

                var entity = await _tarifaRepository.GetEntityByIdAsync(dto.Id);
                if (!EntityValidationHelper.ValidateEntityExists(entity, "Tarifa", out msg))
                    return OperationResult<TarifaDTO>.Fail(msg);

                if (!ValidationHelper.Required(dto.Tipo, "Tipo", out msg))
                    return OperationResult<TarifaDTO>.Fail(msg);
                if (!ValidationHelper.MaxLength(dto.Tipo, 100, "Tipo", out msg))
                    return OperationResult<TarifaDTO>.Fail(msg);
                if (!ValidationHelper.MaxLength(dto.Descripcion, 250, "Descripción", out msg))
                    return OperationResult<TarifaDTO>.Fail(msg);

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
            }, "Error interno al actualizar la tarifa.");
        }

        public async Task<OperationResult<bool>> RemoveAsync(DeleteTarifaDTO dto)
        {
            return await ExecuteOperationAsync(async () =>
            {
                if (!ValidationHelper.IsValidId(dto.Id, "Tarifa", out var msg))
                    return OperationResult<bool>.Fail(msg);

                var entity = await _tarifaRepository.GetEntityByIdAsync(dto.Id);
                if (!EntityValidationHelper.ValidateEntityExists(entity, "Tarifa", out msg))
                    return OperationResult<bool>.Fail(msg);

                var delOp = await _tarifaRepository.DeleteEntityAsync(entity);
                if (!delOp.Success)
                    return OperationResult<bool>.Fail(delOp.Message);

                return OperationResult<bool>.Ok(true, "Tarifa eliminada correctamente.");
            }, "Error interno al eliminar la tarifa.");
        }
    }
}
