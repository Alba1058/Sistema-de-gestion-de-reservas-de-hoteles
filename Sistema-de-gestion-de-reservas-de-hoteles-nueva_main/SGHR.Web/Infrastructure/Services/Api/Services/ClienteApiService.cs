using Microsoft.Extensions.Logging;
using SGHR.Application.DTOs.Clientes.Cliente;
using SGHR.Domain.Base;
using SGHR.Web.Infrastructure.Services.Api.Base;
using SGHR.Web.Infrastructure.Services.Api.Interfaces;

namespace SGHR.Web.Infrastructure.Services.Api.Services
{
    public class ClienteApiService : BaseApiService<ClienteDTO, ClienteCreateDTO, ClienteUpdateDTO, ClienteDeleteDTO>, IClienteApiService
    {
        public ClienteApiService(IHttpClientFactory httpClientFactory, ILogger<ClienteApiService> logger)
            : base(httpClientFactory, logger)
        {
        }

        protected override string EntityName => "Cliente";
        protected override string BaseEndpoint => "Cliente";
    }
}

