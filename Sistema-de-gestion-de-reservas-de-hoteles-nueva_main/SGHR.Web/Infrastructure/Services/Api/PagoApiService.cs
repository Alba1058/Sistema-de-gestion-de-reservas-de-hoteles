using Microsoft.Extensions.Logging;
using SGHR.Application.DTOs.Reservas.Pago;
using SGHR.Domain.Base;
using SGHR.Web.Infrastructure.Services.Api.Base;
using SGHR.Web.Infrastructure.Services.Api.Interfaces;

namespace SGHR.Web.Infrastructure.Services.Api
{
    public class PagoApiService : BaseApiService<PagoDTO, CreatePagoDTO, UpdatePagoDTO, DeletePagoDTO>, IPagoApiService
    {
        public PagoApiService(HttpClient httpClient, ILogger<PagoApiService> logger)
            : base(httpClient, logger)
        {
        }

        protected override string EntityName => "Pago";
        protected override string BaseEndpoint => "Pago";
    }
}

