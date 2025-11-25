using SGHR.Application.DTOs.Reservas.Pago;
using SGHR.Application.Mappers.Base;
using SGHR.Domain.Entities.Reservas;

namespace SGHR.Application.Mappers.Reservas
{
    public static class PagoMapper
    {
        public static PagoDTO ToPagoDto(Pago p)
        {
            ArgumentNullException.ThrowIfNull(p);

            return new PagoDTO
            {
                Id = p.Id,
                IdReserva = p.IdReserva,
                Monto = p.Monto,
                FechaPago = p.FechaPago,
                Metodo = MapperHelper.Clean(p.Metodo),
                Confirmado = p.Confirmado,
                Estado = !p.IsDeleted
            };
        }

        public static Pago CreatePagoEntity(CreatePagoDTO dto, string? usuario = null)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var entity = new Pago
            {
                IdReserva = dto.IdReserva,
                Monto = dto.Monto,
                FechaPago = dto.FechaPago,
                Metodo = MapperHelper.Clean(dto.Metodo),
                Confirmado = dto.Confirmado
            };

            MapperHelper.SetCreationFields(entity, usuario);
            return entity;
        }

        public static void UpdatePagoFromDto(Pago entity, UpdatePagoDTO dto, string? usuario = null)
        {
            ArgumentNullException.ThrowIfNull(entity);
            ArgumentNullException.ThrowIfNull(dto);

            entity.IdReserva = dto.IdReserva;
            entity.Monto = dto.Monto;
            entity.FechaPago = dto.FechaPago;
            entity.Metodo = string.IsNullOrWhiteSpace(dto.Metodo) ? entity.Metodo : MapperHelper.Clean(dto.Metodo);
            entity.Confirmado = dto.Confirmado;

            MapperHelper.SetAuditFields(entity, usuario);
        }
    }
}
