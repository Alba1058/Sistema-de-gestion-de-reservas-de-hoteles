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
    public sealed class HabitacionService : IHabitacionService
    {
        private readonly IHabitacionRepository _habitacionRepository;
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IPisoRepository _pisoRepository;
        private readonly ILogger<HabitacionService> _logger;

        public HabitacionService(
            IHabitacionRepository habitacionRepository,
            ICategoriaRepository categoriaRepository,
            IPisoRepository pisoRepository,
            ILogger<HabitacionService> logger)
        {
            _habitacionRepository = habitacionRepository;
            _categoriaRepository = categoriaRepository;
            _pisoRepository = pisoRepository;
            _logger = logger;
        }

        public async Task<OperationResult<List<HabitacionDTO>>> GetAllAsync()
        {
            try
            {
                var habitaciones = await _habitacionRepository.GetAllAsync();

                var dtoList = habitaciones
                    .Where(h => !h.IsDeleted)
                    .Select(HabitacionMapper.ToHabitacionDto)
                    .ToList();

                return OperationResult<List<HabitacionDTO>>.Ok(dtoList, "Habitaciones obtenidas correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo habitaciones");
                return OperationResult<List<HabitacionDTO>>.Fail("Error al obtener las habitaciones.");
            }
        }

        public async Task<OperationResult<HabitacionDTO>> GetByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return OperationResult<HabitacionDTO>.Fail("El ID de la habitación no es válido.");

                var entity = await _habitacionRepository.GetEntityByIdAsync(id);
                if (entity == null)
                    return OperationResult<HabitacionDTO>.Fail("Habitación no encontrada.");

                var dto = HabitacionMapper.ToHabitacionDto(entity);
                return OperationResult<HabitacionDTO>.Ok(dto, "Habitación obtenida correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo habitación por Id");
                return OperationResult<HabitacionDTO>.Fail("Error interno al obtener la habitación.");
            }
        }

        public async Task<OperationResult<HabitacionDTO>> CreateAsync(CreateHabitacionDTO dto)
        {
            try
            {
                // Validaciones 
                if (!ValidationHelper.NotNull(dto, "Habitación", out var msg)) return OperationResult<HabitacionDTO>.Fail(msg);
                if (dto.Numero <= 0) return OperationResult<HabitacionDTO>.Fail("El número de habitación debe ser mayor que 0.");
                if (dto.IdCategoria <= 0) return OperationResult<HabitacionDTO>.Fail("La categoría es obligatoria.");
                if (dto.IdPiso <= 0) return OperationResult<HabitacionDTO>.Fail("El piso es obligatorio.");
                if (dto.PrecioBase <= 0) return OperationResult<HabitacionDTO>.Fail("El precio base debe ser mayor que 0.");
                if (!ValidationHelper.MaxLength(dto.Descripcion, 250, "Descripción", out msg)) return OperationResult<HabitacionDTO>.Fail(msg);

                // Validacion de entidades relacionadas
                var categoria = await _categoriaRepository.GetEntityByIdAsync(dto.IdCategoria);
                if (categoria == null || categoria.IsDeleted)
                    return OperationResult<HabitacionDTO>.Fail("La categoría especificada no existe.");

                var piso = await _pisoRepository.GetEntityByIdAsync(dto.IdPiso);
                if (piso == null || piso.IsDeleted)
                    return OperationResult<HabitacionDTO>.Fail("El piso especificado no existe.");

                var existeNumero = await _habitacionRepository.ExistsAsync(h =>
                    h.Numero == dto.Numero &&
                    h.IdPiso == dto.IdPiso &&
                    !h.IsDeleted);

                if (existeNumero)
                    return OperationResult<HabitacionDTO>.Fail("Ya existe una habitación con ese número en el mismo piso.");

                // Validacion enum EstadoHabitacion
                if (!Enum.IsDefined(typeof(EstadoHabitacion), dto.EstadoHabitacion))
                    return OperationResult<HabitacionDTO>.Fail("El estado de la habitación no es válido.");

                var entity = HabitacionMapper.CreateHabitacionEntity(dto, usuario: "sistema");
                var saveResult = await _habitacionRepository.SaveEntityAsync(entity);

                if (!saveResult.Success)
                    return OperationResult<HabitacionDTO>.Fail(saveResult.Message);

                var createdDto = HabitacionMapper.ToHabitacionDto(saveResult.Data!);
                return OperationResult<HabitacionDTO>.Ok(createdDto, "Habitación creada correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear habitación");
                return OperationResult<HabitacionDTO>.Fail("Error interno al crear la habitación.");
            }
        }

        public async Task<OperationResult<HabitacionDTO>> UpdateAsync(UpdateHabitacionDTO dto)
        {
            try
            {
                if (dto.Id <= 0) return OperationResult<HabitacionDTO>.Fail("El ID no es válido.");
                if (!ValidationHelper.NotNull(dto, "Habitación", out var msg)) return OperationResult<HabitacionDTO>.Fail(msg);

                var entity = await _habitacionRepository.GetEntityByIdAsync(dto.Id);
                if (entity == null) return OperationResult<HabitacionDTO>.Fail("Habitación no encontrada.");

                // Validaciones
                if (dto.Numero <= 0) return OperationResult<HabitacionDTO>.Fail("El número de habitación debe ser mayor que 0.");
                if (dto.IdCategoria <= 0) return OperationResult<HabitacionDTO>.Fail("La categoría es obligatoria.");
                if (dto.IdPiso <= 0) return OperationResult<HabitacionDTO>.Fail("El piso es obligatorio.");
                if (dto.PrecioBase <= 0) return OperationResult<HabitacionDTO>.Fail("El precio base debe ser mayor que 0.");
                if (!ValidationHelper.MaxLength(dto.Descripcion, 250, "Descripción", out msg)) return OperationResult<HabitacionDTO>.Fail(msg);

                var categoria = await _categoriaRepository.GetEntityByIdAsync(dto.IdCategoria);
                if (categoria == null || categoria.IsDeleted)
                    return OperationResult<HabitacionDTO>.Fail("La categoría especificada no existe.");

                var piso = await _pisoRepository.GetEntityByIdAsync(dto.IdPiso);
                if (piso == null || piso.IsDeleted)
                    return OperationResult<HabitacionDTO>.Fail("El piso especificado no existe.");

                var existeNumero = await _habitacionRepository.ExistsAsync(h =>
                    h.Numero == dto.Numero &&
                    h.IdPiso == dto.IdPiso &&
                    h.Id != dto.Id &&
                    !h.IsDeleted);

                if (existeNumero)
                    return OperationResult<HabitacionDTO>.Fail("Ya existe otra habitación con ese número en el mismo piso.");

                if (!Enum.IsDefined(typeof(EstadoHabitacion), dto.EstadoHabitacion))
                    return OperationResult<HabitacionDTO>.Fail("El estado de la habitación no es válido.");

                HabitacionMapper.UpdateHabitacionFromDto(entity, dto, usuario: "sistema");
                var updateOp = await _habitacionRepository.UpdateEntityAsync(entity);

                if (!updateOp.Success)
                    return OperationResult<HabitacionDTO>.Fail(updateOp.Message);

                var dtoResult = HabitacionMapper.ToHabitacionDto(updateOp.Data!);
                return OperationResult<HabitacionDTO>.Ok(dtoResult, "Habitación actualizada correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar habitación");
                return OperationResult<HabitacionDTO>.Fail("Error interno al actualizar la habitación.");
            }
        }

        public async Task<OperationResult<bool>> RemoveAsync(DeleteHabitacionDTO dto)
        {
            try
            {
                if (dto.Id <= 0) return OperationResult<bool>.Fail("El ID no es válido.");

                var entity = await _habitacionRepository.GetEntityByIdAsync(dto.Id);
                if (entity == null) return OperationResult<bool>.Fail("Habitación no encontrada.");

                var delOp = await _habitacionRepository.DeleteEntityAsync(entity);
                if (!delOp.Success) return OperationResult<bool>.Fail(delOp.Message);

                return OperationResult<bool>.Ok(true, "Habitación eliminada correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar habitación");
                return OperationResult<bool>.Fail("Error interno al eliminar la habitación.");
            }
        }
        public async Task<OperationResult<List<HabitacionDTO>>> GetHabitacionesDisponiblesAsync()
        {
            try
            {
                var habitaciones = await _habitacionRepository.GetHabitacionesDisponiblesAsync();
                var dtoList = habitaciones.Select(HabitacionMapper.ToHabitacionDto).ToList();

                return OperationResult<List<HabitacionDTO>>.Ok(dtoList, "Habitaciones disponibles obtenidas correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener habitaciones disponibles");
                return OperationResult<List<HabitacionDTO>>.Fail("Error al obtener habitaciones disponibles.");
            }
        }
    }
}
