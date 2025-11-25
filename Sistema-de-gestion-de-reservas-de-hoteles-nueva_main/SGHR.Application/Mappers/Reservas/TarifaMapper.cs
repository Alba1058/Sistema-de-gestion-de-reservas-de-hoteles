using SGHR.Application.DTOs.Reservas.Tarifa;
using SGHR.Application.Mappers.Base;
using SGHR.Domain.Entities.Reservas;

namespace SGHR.Application.Mappers.Reservas
{
    public static class TarifaMapper
    {
        public static TarifaDTO ToTarifaDto(Tarifa? entity)
        {
            if (entity == null)
                return new TarifaDTO();

            return new TarifaDTO
            {
                Id = entity.Id,
                Tipo = entity.Tipo,
                Monto = entity.Monto,
                FechaInicio = entity.FechaInicio,
                FechaFin = entity.FechaFin,
                PrecioPorNoche = entity.PrecioPorNoche,
                Descuento = entity.Descuento,
                Descripcion = MapperHelper.Clean(entity.Descripcion),
                IdHabitacion = entity.IdHabitacion,
                Estado = !entity.IsDeleted
            };
        }

        public static Tarifa CreateTarifaEntity(CreateTarifaDTO dto, string? usuario = null)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var entity = new Tarifa
            {
                Tipo = MapperHelper.Clean(dto.Tipo),
                Monto = dto.Monto,
                FechaInicio = dto.FechaInicio,
                FechaFin = dto.FechaFin,
                PrecioPorNoche = dto.PrecioPorNoche,
                Descuento = dto.Descuento,
                Descripcion = MapperHelper.Clean(dto.Descripcion),
                IdHabitacion = dto.IdHabitacion
            };

            MapperHelper.SetCreationFields(entity, usuario);
            return entity;
        }

        public static void UpdateTarifaFromDto(Tarifa entity, UpdateTarifaDTO dto, string? usuario = null)
        {
            if (entity == null || dto == null) return;

            entity.Tipo = string.IsNullOrWhiteSpace(dto.Tipo) ? entity.Tipo : MapperHelper.Clean(dto.Tipo);
            entity.Monto = dto.Monto;
            entity.FechaInicio = dto.FechaInicio;
            entity.FechaFin = dto.FechaFin;
            entity.PrecioPorNoche = dto.PrecioPorNoche;
            entity.Descuento = dto.Descuento;
            entity.Descripcion = string.IsNullOrWhiteSpace(dto.Descripcion) ? entity.Descripcion : MapperHelper.Clean(dto.Descripcion);
            entity.IdHabitacion = dto.IdHabitacion;
            entity.Estado = dto.Estado;

            MapperHelper.SetAuditFields(entity, usuario);
        }
    }
}
