using SGHR.Domain.Base;
using SGHR.Application.DTOs.Reservas.Pago;

namespace SGHR.Application.Interfaces.Reservas
{
    public interface IPagoService : IBaseService<CreatePagoDTO, UpdatePagoDTO, DeletePagoDTO, PagoDTO>
    {
    }
}