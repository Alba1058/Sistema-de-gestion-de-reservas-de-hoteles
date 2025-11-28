using Microsoft.Extensions.Logging;
using SGHR.Application.Base;
using SGHR.Application.DTOs.Configuration.Piso;
using SGHR.Application.Mappers.Configuration;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration;
using SGHR.Application.Interfaces.Configuration;
using SGHR.Persistence.Interfaces.Configuration;

namespace SGHR.Application.Services.Configuration
{
    public class PisoService : BaseService, IPisoService
    {
        private readonly IPisoRepository _repository;

        public PisoService(IPisoRepository repository, ILogger<PisoService> logger)
            : base(logger)
        {
            _repository = repository;
        }

        public async Task<OperationResult<PisoDTO>> CreateAsync(CreatePisoDTO dto)
        {
            return await ExecuteOperationAsync(async () =>
            {
                // Validaciones
                if (!ValidationHelper.NotNull(dto, nameof(CreatePisoDTO), out var msg))
                    return OperationResult<PisoDTO>.Fail(msg);

                if (dto.Numero <= 0)
                    return OperationResult<PisoDTO>.Fail("El número de piso debe ser mayor que cero.");

                if (!ValidationHelper.MaxLength(dto.Descripcion, 250, "Descripción", out msg))
                    return OperationResult<PisoDTO>.Fail(msg);

                //número único
                bool exists = await _repository.ExistsAsync(p => p.Numero == dto.Numero && !p.IsDeleted);
                if (exists)
                    return OperationResult<PisoDTO>.Fail($"Ya existe un piso con el número {dto.Numero}.");

                var entity = PisoMapper.CreatePisoEntity(dto, "sistema");

                var saveResult = await _repository.SaveEntityAsync(entity);
                if (!saveResult.Success)
                    return OperationResult<PisoDTO>.Fail(saveResult.Message);

                var pisoDto = PisoMapper.ToPisoDto(saveResult.Data!);
                return OperationResult<PisoDTO>.Ok(pisoDto, "Piso creado correctamente.");
            }, "Error al crear el piso.");
        }

        public async Task<OperationResult<PisoDTO>> UpdateAsync(UpdatePisoDTO dto)
        {
            return await ExecuteOperationAsync(async () =>
            {
                if (!ValidationHelper.NotNull(dto, nameof(UpdatePisoDTO), out var msg))
                    return OperationResult<PisoDTO>.Fail(msg);

                if (dto.Id <= 0)
                    return OperationResult<PisoDTO>.Fail("El ID del piso es inválido.");

                var entity = await _repository.GetEntityByIdAsync(dto.Id);
                if (entity == null)
                    return OperationResult<PisoDTO>.Fail("El piso no existe.");

                // número duplicado (excepto el mismo piso)
                bool exists = await _repository.ExistsAsync(p => p.Numero == dto.Numero && p.Id != dto.Id && !p.IsDeleted);
                if (exists)
                    return OperationResult<PisoDTO>.Fail($"Ya existe otro piso con el número {dto.Numero}.");

                PisoMapper.UpdatePisoFromDto(entity, dto, "sistema");

                var updateResult = await _repository.UpdateEntityAsync(entity);
                if (!updateResult.Success)
                    return OperationResult<PisoDTO>.Fail(updateResult.Message);

                return OperationResult<PisoDTO>.Ok(PisoMapper.ToPisoDto(updateResult.Data!), "Piso actualizado correctamente.");
            }, "Error al actualizar el piso.");
        }

        public async Task<OperationResult<bool>> RemoveAsync(DeletePisoDTO dto)
        {
            return await ExecuteOperationAsync(async () =>
            {
                if (dto.Id <= 0)
                    return OperationResult<bool>.Fail("El ID del piso es inválido.");

                var entity = await _repository.GetEntityByIdAsync(dto.Id);
                if (entity == null)
                    return OperationResult<bool>.Fail("El piso no existe.");

                var deleteResult = await _repository.DeleteEntityAsync(entity);
                if (!deleteResult.Success)
                    return OperationResult<bool>.Fail(deleteResult.Message);

                return OperationResult<bool>.Ok(true, "Piso eliminado correctamente.");
            }, "Error al eliminar el piso.");
        }

        public async Task<OperationResult<List<PisoDTO>>> GetAllAsync()
        {
            return await GetAllEntitiesAsync<Piso, PisoDTO>(
                _repository.GetAllAsync,
                PisoMapper.ToPisoDto,
                "Pisos");
        }

        public async Task<OperationResult<PisoDTO>> GetByIdAsync(int id)
        {
            return await ExecuteOperationAsync(async () =>
            {
                var entity = await _repository.GetEntityByIdAsync(id);
                if (entity == null || entity.IsDeleted)
                    return OperationResult<PisoDTO>.Fail("Piso no encontrado.");

                return OperationResult<PisoDTO>.Ok(PisoMapper.ToPisoDto(entity));
            }, "Error al obtener el piso.");
        }
    }
}
