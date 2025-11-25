using SGHR.Application.DTOs.Reservas.Pago;
using SGHR.Domain.Base;

namespace SGHR.Web.Infrastructure.Services.Api.Interfaces
{
    public interface IPagoApiService : IApiService<PagoDTO, CreatePagoDTO, UpdatePagoDTO, DeletePagoDTO>
    {
    }
}

