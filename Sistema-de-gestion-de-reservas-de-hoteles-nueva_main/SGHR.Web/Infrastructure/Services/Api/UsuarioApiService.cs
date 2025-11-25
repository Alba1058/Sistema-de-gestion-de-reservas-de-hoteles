using Microsoft.Extensions.Logging;
using SGHR.Application.DTOs.Usuarios.Usuario;
using SGHR.Domain.Base;
using SGHR.Web.Infrastructure.Services.Api.Base;
using SGHR.Web.Infrastructure.Services.Api.Interfaces;

namespace SGHR.Web.Infrastructure.Services.Api
{
    public class UsuarioApiService : BaseApiService<UsuarioDTO, UsuarioCreateDTO, UsuarioUpdateDTO, UsuarioDeleteDTO>, IUsuarioApiService
    {
        public UsuarioApiService(HttpClient httpClient, ILogger<UsuarioApiService> logger)
            : base(httpClient, logger)
        {
        }

        protected override string EntityName => "Usuario";
        protected override string BaseEndpoint => "Usuario";
    }
}

