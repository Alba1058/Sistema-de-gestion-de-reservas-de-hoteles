using Microsoft.Extensions.Logging;
using SGHR.Application.DTOs.Reservas.Habitacion;
using SGHR.Application.Interfaces.Reservas;
using SGHR.Application.Mappers.Reservas;
using SGHR.Application.Base;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Reservas;
using SGHR.Persistence.Interfaces.Reservas;
using SGHR.Persistence.Interfaces.Configuration;
using SGHR.Domain.Enums;

namespace SGHR.Application.Services.Reservas
{
    public sealed class HabitacionService : BaseService, IHabitacionService
    {
        private readonly IHabitacionRepository _habitacionRepository;
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IPisoRepository _pisoRepository;

        public HabitacionService(
            IHabitacionRepository habitacionRepository,
            ICategoriaRepository categoriaRepository,
            IPisoRepository pisoRepository,
            ILogger<HabitacionService> logger) : base(logger)
        {
            _habitacionRepository = habitacionRepository;
            _categoriaRepository = categoriaRepository;
            _pisoRepository = pisoRepository;
        }

        public async Task<OperationResult<List<HabitacionDTO>>> GetAllAsync()
        {
            return await GetAllEntitiesAsync<Habitacion, HabitacionDTO>(
                _habitacionRepository.GetAllAsync,
                HabitacionMapper.ToHabitacionDto,
                "Habitaciones");
        }

        public async Task<OperationResult<HabitacionDTO>> GetByIdAsync(int id)
        {
            return await ExecuteOperationAsync<HabitacionDTO>(async () =>
            {
                if (!ValidationHelper.IsValidId(id, "Habitación", out var msg))
                    return OperationResult<HabitacionDTO>.Fail(msg);

                var entity = await _habitacionRepository.GetEntityByIdAsync(id);
                if (!EntityValidationHelper.ValidateEntityExists(entity, "Habitación", out msg))
                    return OperationResult<HabitacionDTO>.Fail(msg);

                var dto = HabitacionMapper.ToHabitacionDto(entity!);
                return OperationResult<HabitacionDTO>.Ok(dto, "Habitación obtenida correctamente.");
            }, "Error interno al obtener la habitación.");
        }

        public async Task<OperationResult<HabitacionDTO>> CreateAsync(CreateHabitacionDTO dto)
        {
            return await ExecuteOperationAsync<HabitacionDTO>(async () =>
            {
                if (!ValidationHelper.NotNull(dto, "Habitación", out var msg))
                    return OperationResult<HabitacionDTO>.Fail(msg);
                if (dto.Numero <= 0)
                    return OperationResult<HabitacionDTO>.Fail("El número de habitación debe ser mayor que 0.");
                if (dto.IdCategoria <= 0)
                    return OperationResult<HabitacionDTO>.Fail("La categoría es obligatoria.");
                if (dto.IdPiso <= 0)
                    return OperationResult<HabitacionDTO>.Fail("El piso es obligatorio.");
                if (dto.PrecioBase <= 0)
                    return OperationResult<HabitacionDTO>.Fail("El precio base debe ser mayor que 0.");
                if (!ValidationHelper.MaxLength(dto.Descripcion, 250, "Descripción", out msg))
                    return OperationResult<HabitacionDTO>.Fail(msg);

                var categoria = await _categoriaRepository.GetEntityByIdAsync(dto.IdCategoria);
                if (!EntityValidationHelper.ValidateRelatedEntity(categoria, "categoría", out msg))
                    return OperationResult<HabitacionDTO>.Fail(msg);

                var piso = await _pisoRepository.GetEntityByIdAsync(dto.IdPiso);
                if (!EntityValidationHelper.ValidateRelatedEntity(piso, "piso", out msg))
                    return OperationResult<HabitacionDTO>.Fail(msg);

                var (isValidNumero, numeroMsg) = await UniquenessValidationHelper.ValidateUniquenessAsync<Habitacion>(
                    _habitacionRepository.ExistsAsync,
                    h => h.Numero == dto.Numero && h.IdPiso == dto.IdPiso && !h.IsDeleted,
                    "número en el mismo piso",
                    "habitación");
                if (!isValidNumero)
                    return OperationResult<HabitacionDTO>.Fail(numeroMsg);

                if (!EnumValidationHelper.ValidateEnum((EstadoHabitacion)dto.EstadoHabitacion, "estado de la habitación", out msg))
                    return OperationResult<HabitacionDTO>.Fail(msg);

                var entity = HabitacionMapper.CreateHabitacionEntity(dto, usuario: "sistema");
                var saveResult = await _habitacionRepository.SaveEntityAsync(entity);

                if (!saveResult.Success)
                    return OperationResult<HabitacionDTO>.Fail(saveResult.Message);

                var createdDto = HabitacionMapper.ToHabitacionDto(saveResult.Data!);
                return OperationResult<HabitacionDTO>.Ok(createdDto, "Habitación creada correctamente.");
            }, "Error interno al crear la habitación.");
        }

        public async Task<OperationResult<HabitacionDTO>> UpdateAsync(UpdateHabitacionDTO dto)
        {
            return await ExecuteOperationAsync<HabitacionDTO>(async () =>
            {
                if (!ValidationHelper.IsValidId(dto.Id, "Habitación", out var msg))
                    return OperationResult<HabitacionDTO>.Fail(msg);
                if (!ValidationHelper.NotNull(dto, "Habitación", out msg))
                    return OperationResult<HabitacionDTO>.Fail(msg);

                var entity = await _habitacionRepository.GetEntityByIdAsync(dto.Id);
                if (!EntityValidationHelper.ValidateEntityExists(entity, "Habitación", out msg))
                    return OperationResult<HabitacionDTO>.Fail(msg);

                if (dto.Numero <= 0)
                    return OperationResult<HabitacionDTO>.Fail("El número de habitación debe ser mayor que 0.");
                if (dto.IdCategoria <= 0)
                    return OperationResult<HabitacionDTO>.Fail("La categoría es obligatoria.");
                if (dto.IdPiso <= 0)
                    return OperationResult<HabitacionDTO>.Fail("El piso es obligatorio.");
                if (dto.PrecioBase <= 0)
                    return OperationResult<HabitacionDTO>.Fail("El precio base debe ser mayor que 0.");
                if (!ValidationHelper.MaxLength(dto.Descripcion, 250, "Descripción", out msg))
                    return OperationResult<HabitacionDTO>.Fail(msg);

                var categoria = await _categoriaRepository.GetEntityByIdAsync(dto.IdCategoria);
                if (!EntityValidationHelper.ValidateRelatedEntity(categoria, "categoría", out msg))
                    return OperationResult<HabitacionDTO>.Fail(msg);

                var piso = await _pisoRepository.GetEntityByIdAsync(dto.IdPiso);
                if (!EntityValidationHelper.ValidateRelatedEntity(piso, "piso", out msg))
                    return OperationResult<HabitacionDTO>.Fail(msg);

                var (isValidNumero, numeroMsg) = await UniquenessValidationHelper.ValidateUniquenessForUpdateAsync<Habitacion>(
                    _habitacionRepository.ExistsAsync,
                    h => h.Numero == dto.Numero && h.IdPiso == dto.IdPiso && h.Id != dto.Id && !h.IsDeleted,
                    "número en el mismo piso",
                    "habitación");
                if (!isValidNumero)
                    return OperationResult<HabitacionDTO>.Fail(numeroMsg);

                if (!EnumValidationHelper.ValidateEnum((EstadoHabitacion)dto.EstadoHabitacion, "estado de la habitación", out msg))
                    return OperationResult<HabitacionDTO>.Fail(msg);

                HabitacionMapper.UpdateHabitacionFromDto(entity!, dto, usuario: "sistema");
                var updateOp = await _habitacionRepository.UpdateEntityAsync(entity!);

                if (!updateOp.Success)
                    return OperationResult<HabitacionDTO>.Fail(updateOp.Message);

                var dtoResult = HabitacionMapper.ToHabitacionDto(updateOp.Data!);
                return OperationResult<HabitacionDTO>.Ok(dtoResult, "Habitación actualizada correctamente.");
            }, "Error interno al actualizar la habitación.");
        }

        public async Task<OperationResult<bool>> RemoveAsync(DeleteHabitacionDTO dto)
        {
            return await ExecuteOperationAsync(async () =>
            {
                if (!ValidationHelper.IsValidId(dto.Id, "Habitación", out var msg))
                    return OperationResult<bool>.Fail(msg);

                var entity = await _habitacionRepository.GetEntityByIdAsync(dto.Id);
                if (!EntityValidationHelper.ValidateEntityExists(entity, "Habitación", out msg))
                    return OperationResult<bool>.Fail(msg);

                var delOp = await _habitacionRepository.DeleteEntityAsync(entity!);
                if (!delOp.Success)
                    return OperationResult<bool>.Fail(delOp.Message);

                return OperationResult<bool>.Ok(true, "Habitación eliminada correctamente.");
            }, "Error interno al eliminar la habitación.");
        }
        public async Task<OperationResult<List<HabitacionDTO>>> GetHabitacionesDisponiblesAsync()
        {
            return await ExecuteOperationAsync<List<HabitacionDTO>>(async () =>
            {
                var habitaciones = await _habitacionRepository.GetHabitacionesDisponiblesAsync();
                var dtoList = habitaciones.Select(HabitacionMapper.ToHabitacionDto).ToList();

                return OperationResult<List<HabitacionDTO>>.Ok(dtoList, "Habitaciones disponibles obtenidas correctamente.");
            }, "Error al obtener habitaciones disponibles.");
        }
    }
}
