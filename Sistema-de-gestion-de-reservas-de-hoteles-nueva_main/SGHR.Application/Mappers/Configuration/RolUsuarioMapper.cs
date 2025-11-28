using SGHR.Application.DTOs.Configuration.RolUsuario;
using SGHR.Application.Mappers.Base;
using SGHR.Domain.Entities.Configuration;

namespace SGHR.Application.Mappers.Configuration
{
    public static class RolUsuarioMapper
    {
        public static RolUsuarioDTO ToRolUsuarioDto(RolUsuario r)
        {
            ArgumentNullException.ThrowIfNull(r);

            return new RolUsuarioDTO
            {
                Id = r.Id,
                Nombre = r.Nombre,
                Descripcion = r.Descripcion,
                Estado = r.Estado
            };
        }

        public static RolUsuario CreateRolUsuarioEntity(CreateRolUsuarioDTO dto, string? usuario = null)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var entity = new RolUsuario
            {
                Nombre = MapperHelper.Clean(dto.Nombre),
                Descripcion = MapperHelper.Clean(dto.Descripcion),
                Estado = dto.Estado
            };

            MapperHelper.SetCreationFields(entity, usuario);
            return entity;
        }

        public static void UpdateRolUsuarioFromDto(RolUsuario entity, UpdateRolUsuarioDTO dto, string? usuario = null)
        {
            if (entity == null || dto == null) return;

            entity.Nombre = string.IsNullOrWhiteSpace(dto.Nombre) ? entity.Nombre : MapperHelper.Clean(dto.Nombre);
            entity.Descripcion = string.IsNullOrWhiteSpace(dto.Descripcion) ? entity.Descripcion : MapperHelper.Clean(dto.Descripcion);
            entity.Estado = dto.Estado;
            entity.IsDeleted = !dto.Estado;

            MapperHelper.SetAuditFields(entity, usuario);
        }
    }
}
