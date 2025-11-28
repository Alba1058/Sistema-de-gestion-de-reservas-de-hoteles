using Microsoft.Extensions.Logging;
using SGHR.Application.DTOs.Reservas.Tarifa;
using SGHR.Domain.Base;
using SGHR.Web.Infrastructure.Services.Api.Base;
using SGHR.Web.Infrastructure.Services.Api.Interfaces;

namespace SGHR.Web.Infrastructure.Services.Api.Services
{
    public class TarifaApiService : BaseApiService<TarifaDTO, CreateTarifaDTO, UpdateTarifaDTO, DeleteTarifaDTO>, ITarifaApiService
    {
        public TarifaApiService(IHttpClientFactory httpClientFactory, ILogger<TarifaApiService> logger)
            : base(httpClientFactory, logger)
        {
        }

        protected override string EntityName => "Tarifa";
        protected override string BaseEndpoint => "Tarifa";
    }
}

