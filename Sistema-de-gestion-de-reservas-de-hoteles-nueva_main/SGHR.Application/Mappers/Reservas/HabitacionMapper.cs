using SGHR.Application.DTOs.Reservas.Habitacion;
using SGHR.Application.Mappers.Base;
using SGHR.Domain.Entities.Reservas;
using SGHR.Domain.Enums;

namespace SGHR.Application.Mappers.Reservas
{
    public static class HabitacionMapper
    {
        public static HabitacionDTO ToHabitacionDto(Habitacion h)
        {
            ArgumentNullException.ThrowIfNull(h);

            return new HabitacionDTO
            {
                Id = h.Id,
                Numero = h.Numero,
                IdCategoria = h.IdCategoria,
                IdPiso = h.IdPiso,
                EstadoHabitacion = (int)h.EstadoH,
                PrecioBase = h.PrecioBase,
                Descripcion = MapperHelper.Clean(h.Descripcion),
                Estado = !h.IsDeleted
            };
        }

        public static Habitacion CreateHabitacionEntity(CreateHabitacionDTO dto, string? usuario = null)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var entity = new Habitacion
            {
                Numero = dto.Numero,
                IdCategoria = dto.IdCategoria,
                IdPiso = dto.IdPiso,
                EstadoH = Enum.IsDefined(typeof(EstadoHabitacion), dto.EstadoHabitacion)
                    ? (EstadoHabitacion)dto.EstadoHabitacion
                    : EstadoHabitacion.Disponible,
                PrecioBase = dto.PrecioBase,
                Descripcion = MapperHelper.Clean(dto.Descripcion),
                Estado = dto.Estado,
                IsDeleted = !dto.Estado
            };

            MapperHelper.SetCreationFields(entity, usuario);
            return entity;
        }

        public static void UpdateHabitacionFromDto(Habitacion entity, UpdateHabitacionDTO dto, string? usuario = null)
        {
            ArgumentNullException.ThrowIfNull(entity);
            ArgumentNullException.ThrowIfNull(dto);

            entity.Numero = dto.Numero != 0 ? dto.Numero : entity.Numero;
            entity.IdCategoria = dto.IdCategoria != 0 ? dto.IdCategoria : entity.IdCategoria;
            entity.IdPiso = dto.IdPiso != 0 ? dto.IdPiso : entity.IdPiso;

            if (Enum.IsDefined(typeof(EstadoHabitacion), dto.EstadoHabitacion))
                entity.EstadoH = (EstadoHabitacion)dto.EstadoHabitacion;

            entity.PrecioBase = dto.PrecioBase != 0 ? dto.PrecioBase : entity.PrecioBase;
            entity.Descripcion = string.IsNullOrWhiteSpace(dto.Descripcion)
                ? entity.Descripcion
                : MapperHelper.Clean(dto.Descripcion);

            entity.Estado = dto.Estado;
            entity.IsDeleted = !dto.Estado;

            MapperHelper.SetAuditFields(entity, usuario);
        }
    }
}
