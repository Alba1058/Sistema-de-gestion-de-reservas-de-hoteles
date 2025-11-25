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
    }
}
