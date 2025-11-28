using Microsoft.Extensions.Logging;
using SGHR.Application.DTOs.Configuration.Piso;
using SGHR.Domain.Base;
using SGHR.Web.Infrastructure.Services.Api.Base;
using SGHR.Web.Infrastructure.Services.Api.Interfaces;

namespace SGHR.Web.Infrastructure.Services.Api.Services
{
    public class PisoApiService : BaseApiService<PisoDTO, CreatePisoDTO, UpdatePisoDTO, DeletePisoDTO>, IPisoApiService
    {
        public PisoApiService(IHttpClientFactory httpClientFactory, ILogger<PisoApiService> logger)
            : base(httpClientFactory, logger)
        {
        }

        protected override string EntityName => "Piso";
        protected override string BaseEndpoint => "Piso";
    }
}

