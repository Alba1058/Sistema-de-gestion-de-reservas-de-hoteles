using Microsoft.Extensions.Logging;
using SGHR.Application.Base;
using SGHR.Application.DTOs.Configuration.Categoria;
using SGHR.Application.Interfaces.Configuration;
using SGHR.Application.Mappers.Configuration;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration;
using SGHR.Persistence.Interfaces.Configuration;

namespace SGHR.Application.Services.Configuration
{
    public class CategoriaService : BaseService, ICategoriaService
    {
        private readonly ICategoriaRepository _repository;

        public CategoriaService(ICategoriaRepository repository, ILogger<CategoriaService> logger)
            : base(logger)
        {
            _repository = repository;
        }

        public async Task<OperationResult<List<CategoriaDTO>>> GetAllAsync()
        {
            return await GetAllEntitiesAsync<Categoria, CategoriaDTO>(
                _repository.GetAllAsync,
                CategoriaMapper.ToCategoriaDto,
                "Categorías");
        }

        public async Task<OperationResult<CategoriaDTO>> GetByIdAsync(int id) =>
            await ExecuteOperationAsync(async () =>
            {
                var entity = await _repository.GetEntityByIdAsync(id);
                if (entity == null || entity.IsDeleted)
                    return OperationResult<CategoriaDTO>.Fail("La categoría no existe.");

                return OperationResult<CategoriaDTO>.Ok(CategoriaMapper.ToCategoriaDto(entity));
            }, "Error al obtener la categoría por ID.");

        public async Task<OperationResult<CategoriaDTO>> CreateAsync(CreateCategoriaDTO dto) =>
            await ExecuteOperationAsync(async () =>
            {
                if (!ValidationHelper.Required(dto.Nombre, "Nombre", out var message))
                    return OperationResult<CategoriaDTO>.Fail(message);

                if (!ValidationHelper.MaxLength(dto.Nombre, 100, "Nombre", out message))
                    return OperationResult<CategoriaDTO>.Fail(message);

                // duplicados
                if (await _repository.ExistsAsync(c => c.Nombre == dto.Nombre))
                    return OperationResult<CategoriaDTO>.Fail("Ya existe una categoría con ese nombre.");

                var entity = CategoriaMapper.CreateCategoriaEntity(dto);

                var result = await _repository.SaveEntityAsync(entity);
                if (!result.Success)
                    return OperationResult<CategoriaDTO>.Fail(result.Message ?? "Error al crear la categoría.");

                var dtoResult = CategoriaMapper.ToCategoriaDto(result.Data!);
                return OperationResult<CategoriaDTO>.Ok(dtoResult, "Categoría creada exitosamente.");
            }, "Error al crear la categoría.");

        public async Task<OperationResult<CategoriaDTO>> UpdateAsync(UpdateCategoriaDTO dto) =>
            await ExecuteOperationAsync(async () =>
            {
                var entity = await _repository.GetEntityByIdAsync(dto.Id);
                if (entity == null)
                    return OperationResult<CategoriaDTO>.Fail("La categoría no existe.");

                if (!ValidationHelper.Required(dto.Nombre, "Nombre", out var message))
                    return OperationResult<CategoriaDTO>.Fail(message);

                if (!ValidationHelper.MaxLength(dto.Nombre, 100, "Nombre", out message))
                    return OperationResult<CategoriaDTO>.Fail(message);

                if (await _repository.ExistsAsync(c => c.Nombre == dto.Nombre && c.Id != dto.Id))
                    return OperationResult<CategoriaDTO>.Fail("Ya existe otra categoría con ese nombre.");

                CategoriaMapper.UpdateCategoriaFromDto(entity, dto);
                var result = await _repository.UpdateEntityAsync(entity);

                if (!result.Success)
                    return OperationResult<CategoriaDTO>.Fail(result.Message ?? "Error al actualizar la categoría.");

                var dtoResult = CategoriaMapper.ToCategoriaDto(result.Data!);
                return OperationResult<CategoriaDTO>.Ok(dtoResult, "Categoría actualizada correctamente.");
            }, "Error al actualizar la categoría.");

        public async Task<OperationResult<bool>> RemoveAsync(DeleteCategoriaDTO dto) =>
            await ExecuteOperationAsync(async () =>
            {
                var entity = await _repository.GetEntityByIdAsync(dto.Id);
                if (entity == null)
                    return OperationResult<bool>.Fail("La categoría no existe.");

                if (entity.IsDeleted)
                    return OperationResult<bool>.Fail("La categoría ya está eliminada.");

                var result = await _repository.DeleteEntityAsync(entity);
                return result.Success
                    ? OperationResult<bool>.Ok(true, "Categoría eliminada correctamente.")
                    : OperationResult<bool>.Fail(result.Message ?? "Error al eliminar la categoría.");
            }, "Error al eliminar la categoría.");

        public async Task<OperationResult<List<CategoriaDTO>>> GetCategoriasActivasAsync() =>
            await ExecuteOperationAsync(async () =>
            {
                var categorias = await _repository.GetCategoriasActivas();
                var dtoList = categorias.Select(CategoriaMapper.ToCategoriaDto).ToList();
                return OperationResult<List<CategoriaDTO>>.Ok(dtoList);
            }, "Error al obtener las categorías activas.");
    }
}
