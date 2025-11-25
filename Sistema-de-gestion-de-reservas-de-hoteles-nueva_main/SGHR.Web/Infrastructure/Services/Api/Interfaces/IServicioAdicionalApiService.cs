using SGHR.Application.DTOs.Reservas.ServicioAdicional;
using SGHR.Domain.Base;

namespace SGHR.Web.Infrastructure.Services.Api.Interfaces
{
    public interface IServicioAdicionalApiService : IApiService<ServicioAdicionalDTO, CreateServicioAdicionalDTO, UpdateServicioAdicionalDTO, DeleteServicioAdicionalDTO>
    {
    }
}

