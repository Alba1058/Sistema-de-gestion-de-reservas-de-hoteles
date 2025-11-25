using Microsoft.Extensions.Logging;
using SGHR.Application.DTOs.Configuration.RolUsuario;
using SGHR.Domain.Base;
using SGHR.Web.Infrastructure.Services.Api.Base;
using SGHR.Web.Infrastructure.Services.Api.Interfaces;

namespace SGHR.Web.Infrastructure.Services.Api
{
    public class RolUsuarioApiService : BaseApiService<RolUsuarioDTO, CreateRolUsuarioDTO, UpdateRolUsuarioDTO, DeleteRolUsuarioDTO>, IRolUsuarioApiService
    {
        public RolUsuarioApiService(HttpClient httpClient, ILogger<RolUsuarioApiService> logger)
            : base(httpClient, logger)
        {
        }

        protected override string EntityName => "Rol de Usuario";
        protected override string BaseEndpoint => "RolUsuario";
    }
}

