using SGHR.Application.DTOs.Reservas.ServicioAdicional;
using SGHR.Application.Mappers.Base;
using SGHR.Domain.Entities.Reservas;

namespace SGHR.Application.Mappers.Reservas
{
    public static class ServicioAdicionalMapper
    {
        public static ServicioAdicionalDTO ToServicioAdicionalDto(ServicioAdicional s)
        {
            ArgumentNullException.ThrowIfNull(s);

            return new ServicioAdicionalDTO
            {
                Id = s.Id,
                Nombre = s.Nombre,
                Precio = s.Precio,
                Descripcion = s.Descripcion,
                Estado = !s.IsDeleted
            };
        }

        public static ServicioAdicional CreateServicioAdicionalEntity(CreateServicioAdicionalDTO dto, string? usuario = null)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var entity = new ServicioAdicional
            {
                Nombre = MapperHelper.Clean(dto.Nombre),
                Precio = dto.Precio,
                Descripcion = MapperHelper.Clean(dto.Descripcion),
                Estado = dto.Estado,
                IsDeleted = !dto.Estado
            };

            MapperHelper.SetCreationFields(entity, usuario);
            return entity;
        }

        public static void UpdateServicioAdicionalFromDto(ServicioAdicional entity, UpdateServicioAdicionalDTO dto, string? usuario = null)
        {
            ArgumentNullException.ThrowIfNull(entity);
            ArgumentNullException.ThrowIfNull(dto);

            entity.Nombre = string.IsNullOrWhiteSpace(dto.Nombre) ? entity.Nombre : MapperHelper.Clean(dto.Nombre);
            entity.Precio = dto.Precio;
            entity.Descripcion = string.IsNullOrWhiteSpace(dto.Descripcion) ? entity.Descripcion : MapperHelper.Clean(dto.Descripcion);
            entity.Estado = dto.Estado;
            entity.IsDeleted = !dto.Estado;

            MapperHelper.SetAuditFields(entity, usuario);
        }
    }
}
