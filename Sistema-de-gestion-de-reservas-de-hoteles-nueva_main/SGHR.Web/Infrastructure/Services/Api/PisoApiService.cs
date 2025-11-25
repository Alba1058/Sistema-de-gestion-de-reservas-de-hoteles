using Microsoft.Extensions.Logging;
using SGHR.Application.DTOs.Configuration.Piso;
using SGHR.Domain.Base;
using SGHR.Web.Infrastructure.Services.Api.Base;
using SGHR.Web.Infrastructure.Services.Api.Interfaces;

namespace SGHR.Web.Infrastructure.Services.Api
{
    public class PisoApiService : BaseApiService<PisoDTO, CreatePisoDTO, UpdatePisoDTO, DeletePisoDTO>, IPisoApiService
    {
        public PisoApiService(HttpClient httpClient, ILogger<PisoApiService> logger)
            : base(httpClient, logger)
        {
        }

        protected override string EntityName => "Piso";
        protected override string BaseEndpoint => "Piso";
    }
}

