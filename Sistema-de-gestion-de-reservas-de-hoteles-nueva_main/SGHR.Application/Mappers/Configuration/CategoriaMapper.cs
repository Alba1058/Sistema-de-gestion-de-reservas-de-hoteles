using SGHR.Application.DTOs.Configuration.Categoria;
using SGHR.Domain.Entities.Configuration;
using SGHR.Application.Mappers.Base;

namespace SGHR.Application.Mappers.Configuration
{
    public static class CategoriaMapper
    {
        public static CategoriaDTO ToCategoriaDto(Categoria? entity)
        {
            if (entity == null)
                return new CategoriaDTO();

            return new CategoriaDTO
            {
                Id = entity.Id,
                Nombre = entity.Nombre,
                Descripcion = entity.Descripcion,
                Estado = !entity.IsDeleted
            };
        }

        public static Categoria CreateCategoriaEntity(CreateCategoriaDTO dto, string? usuario = null)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var entity = new Categoria
            {
                Nombre = MapperHelper.Clean(dto.Nombre),
                Descripcion = MapperHelper.Clean(dto.Descripcion),
                IsDeleted = false
            };

            MapperHelper.SetCreationFields(entity, usuario);
            return entity;
        }

        public static void UpdateCategoriaFromDto(Categoria entity, UpdateCategoriaDTO dto, string? usuario = null)
        {
            if (entity == null || dto == null) return;

            entity.Nombre = dto.Nombre?.Trim() ?? entity.Nombre;
            entity.Descripcion = dto.Descripcion?.Trim() ?? entity.Descripcion;
            entity.IsDeleted = !dto.Estado;

            MapperHelper.SetAuditFields(entity, usuario);
        }
    }
}
