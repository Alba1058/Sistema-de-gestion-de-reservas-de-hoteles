using Microsoft.Extensions.Logging;
using SGHR.Application.Base;
using SGHR.Application.DTOs.Reservas.ServicioAdicional;
using SGHR.Application.Interfaces.Reservas;
using SGHR.Application.Mappers.Reservas;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Reservas;
using SGHR.Persistence.Interfaces.Reservas;

namespace SGHR.Application.Services.Reservas
{
    public class ServicioAdicionalService : BaseService, IServicioAdicionalService
    {
        private readonly IServicioAdicionalRepository _repository;

        public ServicioAdicionalService(IServicioAdicionalRepository repository, ILogger<ServicioAdicionalService> logger)
            : base(logger)
        {
            _repository = repository;
        }

        public async Task<OperationResult<ServicioAdicionalDTO>> CreateAsync(CreateServicioAdicionalDTO dto)
        {
            return await ExecuteOperationAsync(async () =>
            {
                // Validaciones
                if (!ValidationHelper.Required(dto.Nombre, nameof(dto.Nombre), out var msg))
                    return OperationResult<ServicioAdicionalDTO>.Fail(msg);

                if (!ValidationHelper.MaxLength(dto.Nombre, 100, nameof(dto.Nombre), out msg))
                    return OperationResult<ServicioAdicionalDTO>.Fail(msg);

                if (!ValidationHelper.MaxLength(dto.Descripcion, 250, nameof(dto.Descripcion), out msg))
                    return OperationResult<ServicioAdicionalDTO>.Fail(msg);

                if (dto.Precio < 0)
                    return OperationResult<ServicioAdicionalDTO>.Fail("El precio no puede ser negativo.");

                var exists = await _repository.ExistsAsync(x => x.Nombre == dto.Nombre && !x.IsDeleted);
                if (exists)
                    return OperationResult<ServicioAdicionalDTO>.Fail("Ya existe un servicio con ese nombre.");

                var entity = ServicioAdicionalMapper.CreateServicioAdicionalEntity(dto);
                var result = await _repository.SaveEntityAsync(entity);
                if (!result.Success)
                    return OperationResult<ServicioAdicionalDTO>.Fail("Error al crear el servicio adicional.");

                var dtoResult = ServicioAdicionalMapper.ToServicioAdicionalDto(result.Data!);
                return OperationResult<ServicioAdicionalDTO>.Ok(dtoResult, "Servicio adicional creado correctamente.");
            },
            "Error al crear el servicio adicional.");
        }

        public async Task<OperationResult<ServicioAdicionalDTO>> UpdateAsync(UpdateServicioAdicionalDTO dto)
        {
            return await ExecuteOperationAsync(async () =>
            {
                if (dto.Id <= 0)
                    return OperationResult<ServicioAdicionalDTO>.Fail("El ID proporcionado no es válido.");

                var entity = await _repository.GetEntityByIdAsync(dto.Id);
                if (entity == null)
                    return OperationResult<ServicioAdicionalDTO>.Fail("El servicio adicional no existe.");

                if (!ValidationHelper.MaxLength(dto.Nombre, 100, nameof(dto.Nombre), out var msg))
                    return OperationResult<ServicioAdicionalDTO>.Fail(msg);

                if (!ValidationHelper.MaxLength(dto.Descripcion, 250, nameof(dto.Descripcion), out msg))
                    return OperationResult<ServicioAdicionalDTO>.Fail(msg);

                if (dto.Precio < 0)
                    return OperationResult<ServicioAdicionalDTO>.Fail("El precio no puede ser negativo.");

                ServicioAdicionalMapper.UpdateServicioAdicionalFromDto(entity, dto);
                var result = await _repository.UpdateEntityAsync(entity);

                if (!result.Success)
                    return OperationResult<ServicioAdicionalDTO>.Fail("Error al actualizar el servicio adicional.");

                var dtoResult = ServicioAdicionalMapper.ToServicioAdicionalDto(result.Data!);
                return OperationResult<ServicioAdicionalDTO>.Ok(dtoResult, "Servicio adicional actualizado correctamente.");
            },
            "Error al actualizar el servicio adicional.");
        }

        public async Task<OperationResult<bool>> RemoveAsync(DeleteServicioAdicionalDTO dto)
        {
            return await ExecuteOperationAsync(async () =>
            {
                var entity = await _repository.GetEntityByIdAsync(dto.Id);
                if (entity == null)
                    return OperationResult<bool>.Fail("El servicio adicional no existe.");

                var result = await _repository.DeleteEntityAsync(entity);
                return result.Success
                    ? OperationResult<bool>.Ok(true, "Servicio adicional eliminado correctamente.")
                    : OperationResult<bool>.Fail("Error al eliminar el servicio adicional.");
            },
            "Error al eliminar el servicio adicional.");
        }

        public async Task<OperationResult<ServicioAdicionalDTO>> GetByIdAsync(int id)
        {
            return await ExecuteOperationAsync(async () =>
            {
                var entity = await _repository.GetEntityByIdAsync(id);
                if (entity == null || entity.IsDeleted)
                    return OperationResult<ServicioAdicionalDTO>.Fail("No se encontró el servicio adicional.");

                var dto = ServicioAdicionalMapper.ToServicioAdicionalDto(entity);
                return OperationResult<ServicioAdicionalDTO>.Ok(dto);
            },
            "Error al obtener el servicio adicional.");
        }

        public async Task<OperationResult<List<ServicioAdicionalDTO>>> GetAllAsync()
        {
            return await ExecuteOperationAsync(async () =>
            {
                var list = await _repository.GetAllAsync();
                var dtoList = list.Where(s => !s.IsDeleted)
                           .Select(ServicioAdicionalMapper.ToServicioAdicionalDto).ToList();
                return OperationResult<List<ServicioAdicionalDTO>>.Ok(dtoList);
            },
            "Error al obtener los servicios adicionales.");
        }

        public async Task<OperationResult<List<ServicioAdicionalDTO>>> GetServiciosDisponiblesAsync()
        {
            return await ExecuteOperationAsync(async () =>
            {
                var disponibles = await _repository.GetServiciosDisponiblesAsync();
                var dtoList = disponibles.Select(ServicioAdicionalMapper.ToServicioAdicionalDto).ToList();
                return OperationResult<List<ServicioAdicionalDTO>>.Ok(dtoList);
            },
            "Error al obtener los servicios disponibles.");
        }
    }
}
