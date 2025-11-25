using SGHR.Application.DTOs.Reservas.Reserva;
using SGHR.Domain.Base;

namespace SGHR.Web.Infrastructure.Services.Api.Interfaces
{
    public interface IReservaApiService : IApiService<ReservaDTO, CreateReservaDTO, UpdateReservaDTO, DeleteReservaDTO>
    {
    }
}

