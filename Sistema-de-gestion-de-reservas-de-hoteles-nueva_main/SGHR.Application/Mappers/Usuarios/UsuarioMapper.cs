using SGHR.Application.DTOs.Usuarios.Usuario;
using SGHR.Application.Mappers.Base;
using SGHR.Domain.Entities.Usuarios;

namespace SGHR.Application.Mappers.Usuarios
{
    public static class UsuarioMapper
    {
        public static UsuarioDTO ToUsuarioDto(Usuario? entity)
        {
            if (entity == null)
                return new UsuarioDTO();

            return new UsuarioDTO
            {
                Id = entity.Id,
                Nombre = entity.Nombre,
                Email = entity.Email,
                RolUsuarioId = entity.RolUsuarioId,
                RolNombre = entity.RolUsuario?.Nombre ?? string.Empty,
                Activo = entity.Estado
            };
        }

        public static Usuario CreateUsuarioEntity(UsuarioCreateDTO dto, string? usuario = null)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var entity = new Usuario
            {
                Nombre = MapperHelper.Clean(dto.Nombre),
                Email = MapperHelper.Clean(dto.Email),
                Contrasena = dto.Contrasena,
                RolUsuarioId = dto.RolUsuarioId,
                Estado = true,
                IsDeleted = false
            };

            MapperHelper.SetCreationFields(entity, usuario);
            return entity;
        }

        public static void UpdateUsuarioFromDto(Usuario entity, UsuarioUpdateDTO dto, string? usuario = null)
        {
            if (entity == null || dto == null) return;

            entity.Nombre = string.IsNullOrWhiteSpace(dto.Nombre) ? entity.Nombre : MapperHelper.Clean(dto.Nombre);
            entity.Email = string.IsNullOrWhiteSpace(dto.Email) ? entity.Email : MapperHelper.Clean(dto.Email);
            entity.Estado = dto.Activo;
            entity.RolUsuarioId = dto.RolUsuarioId;

            MapperHelper.SetAuditFields(entity, usuario);
        }
    }
}
