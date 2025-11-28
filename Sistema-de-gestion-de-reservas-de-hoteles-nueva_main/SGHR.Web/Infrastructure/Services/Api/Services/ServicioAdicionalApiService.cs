using Microsoft.Extensions.Logging;
using SGHR.Application.DTOs.Reservas.ServicioAdicional;
using SGHR.Domain.Base;
using SGHR.Web.Infrastructure.Services.Api.Base;
using SGHR.Web.Infrastructure.Services.Api.Interfaces;

namespace SGHR.Web.Infrastructure.Services.Api.Services
{
    public class ServicioAdicionalApiService : BaseApiService<ServicioAdicionalDTO, CreateServicioAdicionalDTO, UpdateServicioAdicionalDTO, DeleteServicioAdicionalDTO>, IServicioAdicionalApiService
    {
        public ServicioAdicionalApiService(IHttpClientFactory httpClientFactory, ILogger<ServicioAdicionalApiService> logger)
            : base(httpClientFactory, logger)
        {
        }

        protected override string EntityName => "Servicio Adicional";
        protected override string BaseEndpoint => "ServicioAdicional";
    }
}

