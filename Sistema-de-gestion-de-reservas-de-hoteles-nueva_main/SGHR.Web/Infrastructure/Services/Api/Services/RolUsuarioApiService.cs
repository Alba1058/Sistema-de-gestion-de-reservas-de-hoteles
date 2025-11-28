using Microsoft.Extensions.Logging;
using SGHR.Application.DTOs.Configuration.RolUsuario;
using SGHR.Domain.Base;
using SGHR.Web.Infrastructure.Services.Api.Base;
using SGHR.Web.Infrastructure.Services.Api.Interfaces;

namespace SGHR.Web.Infrastructure.Services.Api.Services
{
    public class RolUsuarioApiService : BaseApiService<RolUsuarioDTO, CreateRolUsuarioDTO, UpdateRolUsuarioDTO, DeleteRolUsuarioDTO>, IRolUsuarioApiService
    {
        public RolUsuarioApiService(IHttpClientFactory httpClientFactory, ILogger<RolUsuarioApiService> logger)
            : base(httpClientFactory, logger)
        {
        }

        protected override string EntityName => "Rol de Usuario";
        protected override string BaseEndpoint => "RolUsuario";
    }
}

