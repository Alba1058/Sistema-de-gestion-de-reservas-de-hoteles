using Microsoft.Extensions.Logging;
using SGHR.Application.DTOs.Clientes.Cliente;
using SGHR.Domain.Base;
using SGHR.Web.Infrastructure.Services.Api.Base;
using SGHR.Web.Infrastructure.Services.Api.Interfaces;

namespace SGHR.Web.Infrastructure.Services.Api
{
    public class ClienteApiService : BaseApiService<ClienteDTO, ClienteCreateDTO, ClienteUpdateDTO, ClienteDeleteDTO>, IClienteApiService
    {
        public ClienteApiService(HttpClient httpClient, ILogger<ClienteApiService> logger)
            : base(httpClient, logger)
        {
        }

        protected override string EntityName => "Cliente";
        protected override string BaseEndpoint => "Cliente";
    }
}

