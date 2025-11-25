using SGHR.Application.DTOs.Reservas.Reserva;
using SGHR.Application.Mappers.Base;
using SGHR.Domain.Entities.Reservas;
using SGHR.Domain.Enums;

namespace SGHR.Application.Mappers.Reservas
{
    public static class ReservaMapper
    {
        public static ReservaDTO ToReservaDto(Reserva r)
        {
            ArgumentNullException.ThrowIfNull(r);

            return new ReservaDTO
            {
                Id = r.Id,
                IdCliente = r.IdCliente,
                IdHabitacion = r.IdHabitacion,
                FechaInicio = r.FechaInicio,
                FechaFin = r.FechaFin,
                NumeroHuespedes = r.NumeroHuespedes,
                Total = r.Total,
                EstadoReserva = (int)r.EstadoReserva,
                Estado = !r.IsDeleted
            };
        }

        public static Reserva CreateReservaEntity(CreateReservaDTO dto, string? usuario = null)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var entity = new Reserva
            {
                IdCliente = dto.IdCliente,
                IdHabitacion = dto.IdHabitacion,
                FechaInicio = dto.FechaInicio,
                FechaFin = dto.FechaFin,
                NumeroHuespedes = dto.NumeroHuespedes,
                Total = dto.Total,
                EstadoReserva = Enum.IsDefined(typeof(EstadoReserva), dto.EstadoReserva)
                    ? (EstadoReserva)dto.EstadoReserva
                    : EstadoReserva.Activa
            };

            MapperHelper.SetCreationFields(entity, usuario);
            return entity;
        }

        public static void UpdateReservaFromDto(Reserva entity, UpdateReservaDTO dto, string? usuario = null)
        {
            ArgumentNullException.ThrowIfNull(entity);
            ArgumentNullException.ThrowIfNull(dto);

            entity.IdCliente = dto.IdCliente;
            entity.IdHabitacion = dto.IdHabitacion;
            entity.FechaInicio = dto.FechaInicio;
            entity.FechaFin = dto.FechaFin;
            entity.NumeroHuespedes = dto.NumeroHuespedes;
            entity.Total = dto.Total;

            if (Enum.IsDefined(typeof(EstadoReserva), dto.EstadoReserva))
                entity.EstadoReserva = (EstadoReserva)dto.EstadoReserva;

            MapperHelper.SetAuditFields(entity, usuario);
        }
    }
}
