using Microsoft.Extensions.Logging;
using SGHR.Application.Base;
using SGHR.Application.DTOs.Configuration.RolUsuario;
using SGHR.Application.Interfaces.Configuration;
using SGHR.Application.Mappers.Configuration;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration;
using SGHR.Persistence.Interfaces.Configuration;

namespace SGHR.Application.Services.Configuration
{
    public class RolUsuarioService : BaseService, IRolUsuarioService
    {
        private readonly IRolUsuarioRepository _repository;

        public RolUsuarioService(IRolUsuarioRepository repository, ILogger<RolUsuarioService> logger)
            : base(logger)
        {
            _repository = repository;
        }

        public async Task<OperationResult<List<RolUsuarioDTO>>> GetAllAsync() =>
            await ExecuteOperationAsync(async () =>
            {
                var roles = await _repository.GetAllAsync();
                var dtoList = roles.Where(r => !r.IsDeleted)
                            .Select(RolUsuarioMapper.ToRolUsuarioDto).ToList();
                return OperationResult<List<RolUsuarioDTO>>.Ok(dtoList);
            }, "Error al obtener los roles.");

        public async Task<OperationResult<RolUsuarioDTO>> GetByIdAsync(int id) =>
            await ExecuteOperationAsync(async () =>
            {
                var rol = await _repository.GetEntityByIdAsync(id);
                if (rol == null || rol.IsDeleted)
                    return OperationResult<RolUsuarioDTO>.Fail("El rol no existe.");

                return OperationResult<RolUsuarioDTO>.Ok(RolUsuarioMapper.ToRolUsuarioDto(rol));
            }, "Error al obtener el rol por ID.");

        public async Task<OperationResult<RolUsuarioDTO>> CreateAsync(CreateRolUsuarioDTO dto) =>
            await ExecuteOperationAsync(async () =>
            {
                if (await _repository.ExistsAsync(r => r.Nombre == dto.Nombre))
                    return OperationResult<RolUsuarioDTO>.Fail("Ya existe un rol con ese nombre.");

                var entity = RolUsuarioMapper.CreateRolUsuarioEntity(dto);
                var result = await _repository.SaveEntityAsync(entity);

                if (!result.Success)
                    return OperationResult<RolUsuarioDTO>.Fail(result.Message ?? "Error al crear el rol.");

                var dtoResult = RolUsuarioMapper.ToRolUsuarioDto(result.Data!);
                return OperationResult<RolUsuarioDTO>.Ok(dtoResult, "Rol creado exitosamente.");
            }, "Error al crear el rol.");

        public async Task<OperationResult<RolUsuarioDTO>> UpdateAsync(UpdateRolUsuarioDTO dto) =>
            await ExecuteOperationAsync(async () =>
            {
                var existing = await _repository.GetEntityByIdAsync(dto.Id);
                if (existing == null)
                    return OperationResult<RolUsuarioDTO>.Fail("El rol no existe.");

                if (await _repository.ExistsAsync(r => r.Nombre == dto.Nombre && r.Id != dto.Id))
                    return OperationResult<RolUsuarioDTO>.Fail("Ya existe otro rol con ese nombre.");

                existing.Nombre = dto.Nombre.Trim();
                existing.Descripcion = dto.Descripcion?.Trim();
                existing.IsDeleted = !dto.Estado;

                var result = await _repository.UpdateEntityAsync(existing);
                if (!result.Success)
                    return OperationResult<RolUsuarioDTO>.Fail(result.Message ?? "Error al actualizar el rol.");

                var dtoResult = RolUsuarioMapper.ToRolUsuarioDto(result.Data!);
                return OperationResult<RolUsuarioDTO>.Ok(dtoResult, "Rol actualizado correctamente.");
            }, "Error al actualizar el rol.");

        public async Task<OperationResult<bool>> RemoveAsync(DeleteRolUsuarioDTO dto) =>
            await ExecuteOperationAsync(async () =>
            {
                var entity = await _repository.GetEntityByIdAsync(dto.Id);
                if (entity == null)
                    return OperationResult<bool>.Fail("El rol no existe.");

                var result = await _repository.DeleteEntityAsync(entity);
                return result.Success
                    ? OperationResult<bool>.Ok(true, "Rol eliminado correctamente.")
                    : OperationResult<bool>.Fail(result.Message ?? "Error al eliminar el rol.");
            }, "Error al eliminar el rol.");

        public async Task<OperationResult<List<RolUsuarioDTO>>> GetRolesActivosAsync() =>
            await ExecuteOperationAsync(async () =>
            {
                var roles = await _repository.GetRolesActivosAsync();
                var dtoList = roles.Select(RolUsuarioMapper.ToRolUsuarioDto).ToList();
                return OperationResult<List<RolUsuarioDTO>>.Ok(dtoList);
            }, "Error al obtener los roles activos.");
    }
}
