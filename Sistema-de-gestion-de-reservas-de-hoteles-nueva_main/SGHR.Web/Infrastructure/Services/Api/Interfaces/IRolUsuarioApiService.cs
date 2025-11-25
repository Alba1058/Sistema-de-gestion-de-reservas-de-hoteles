using SGHR.Application.DTOs.Configuration.RolUsuario;
using SGHR.Domain.Base;

namespace SGHR.Web.Infrastructure.Services.Api.Interfaces
{
    public interface IRolUsuarioApiService : IApiService<RolUsuarioDTO, CreateRolUsuarioDTO, UpdateRolUsuarioDTO, DeleteRolUsuarioDTO>
    {
    }
}

