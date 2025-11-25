using Microsoft.Extensions.Logging;
using SGHR.Application.DTOs.Reservas.Tarifa;
using SGHR.Domain.Base;
using SGHR.Web.Infrastructure.Services.Api.Base;
using SGHR.Web.Infrastructure.Services.Api.Interfaces;

namespace SGHR.Web.Infrastructure.Services.Api
{
    public class TarifaApiService : BaseApiService<TarifaDTO, CreateTarifaDTO, UpdateTarifaDTO, DeleteTarifaDTO>, ITarifaApiService
    {
        public TarifaApiService(HttpClient httpClient, ILogger<TarifaApiService> logger)
            : base(httpClient, logger)
        {
        }

        protected override string EntityName => "Tarifa";
        protected override string BaseEndpoint => "Tarifa";
    }
}

