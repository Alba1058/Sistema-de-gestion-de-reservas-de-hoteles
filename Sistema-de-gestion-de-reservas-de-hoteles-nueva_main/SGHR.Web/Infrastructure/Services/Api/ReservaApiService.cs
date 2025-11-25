using Microsoft.Extensions.Logging;
using SGHR.Application.DTOs.Reservas.Reserva;
using SGHR.Domain.Base;
using SGHR.Web.Infrastructure.Services.Api.Base;
using SGHR.Web.Infrastructure.Services.Api.Interfaces;

namespace SGHR.Web.Infrastructure.Services.Api
{
    public class ReservaApiService : BaseApiService<ReservaDTO, CreateReservaDTO, UpdateReservaDTO, DeleteReservaDTO>, IReservaApiService
    {
        public ReservaApiService(HttpClient httpClient, ILogger<ReservaApiService> logger)
            : base(httpClient, logger)
        {
        }

        protected override string EntityName => "Reserva";
        protected override string BaseEndpoint => "Reserva";
    }
}

