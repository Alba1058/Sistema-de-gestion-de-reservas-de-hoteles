using Microsoft.Extensions.Logging;
using SGHR.Application.DTOs.Reservas.Pago;
using SGHR.Domain.Base;
using SGHR.Web.Infrastructure.Services.Api.Base;
using SGHR.Web.Infrastructure.Services.Api.Interfaces;

namespace SGHR.Web.Infrastructure.Services.Api.Services
{
    public class PagoApiService : BaseApiService<PagoDTO, CreatePagoDTO, UpdatePagoDTO, DeletePagoDTO>, IPagoApiService
    {
        public PagoApiService(IHttpClientFactory httpClientFactory, ILogger<PagoApiService> logger)
            : base(httpClientFactory, logger)
        {
        }

        protected override string EntityName => "Pago";
        protected override string BaseEndpoint => "Pago";
    }
}

