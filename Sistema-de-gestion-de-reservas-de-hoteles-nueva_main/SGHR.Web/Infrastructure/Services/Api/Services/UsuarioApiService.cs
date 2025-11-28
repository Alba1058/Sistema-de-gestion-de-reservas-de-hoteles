using Microsoft.Extensions.Logging;
using SGHR.Application.DTOs.Usuarios.Usuario;
using SGHR.Domain.Base;
using SGHR.Web.Infrastructure.Services.Api.Base;
using SGHR.Web.Infrastructure.Services.Api.Interfaces;

namespace SGHR.Web.Infrastructure.Services.Api.Services
{
    public class UsuarioApiService : BaseApiService<UsuarioDTO, UsuarioCreateDTO, UsuarioUpdateDTO, UsuarioDeleteDTO>, IUsuarioApiService
    {
        public UsuarioApiService(IHttpClientFactory httpClientFactory, ILogger<UsuarioApiService> logger)
            : base(httpClientFactory, logger)
        {
        }

        protected override string EntityName => "Usuario";
        protected override string BaseEndpoint => "Usuario";
    }
}

