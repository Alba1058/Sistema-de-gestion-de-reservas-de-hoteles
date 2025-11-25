using SGHR.Application.DTOs.Reservas.Tarifa;
using SGHR.Domain.Base;

namespace SGHR.Web.Infrastructure.Services.Api.Interfaces
{
    public interface ITarifaApiService : IApiService<TarifaDTO, CreateTarifaDTO, UpdateTarifaDTO, DeleteTarifaDTO>
    {
    }
}

