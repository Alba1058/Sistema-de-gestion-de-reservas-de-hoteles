using SGHR.Application.DTOs.Configuration.Piso;
using SGHR.Domain.Base;

namespace SGHR.Web.Infrastructure.Services.Api.Interfaces
{
    public interface IPisoApiService : IApiService<PisoDTO, CreatePisoDTO, UpdatePisoDTO, DeletePisoDTO>
    {
    }
}

