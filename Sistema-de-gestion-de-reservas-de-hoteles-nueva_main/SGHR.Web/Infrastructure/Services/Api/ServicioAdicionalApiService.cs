using Microsoft.Extensions.Logging;
using SGHR.Application.DTOs.Reservas.ServicioAdicional;
using SGHR.Domain.Base;
using SGHR.Web.Infrastructure.Services.Api.Base;
using SGHR.Web.Infrastructure.Services.Api.Interfaces;

namespace SGHR.Web.Infrastructure.Services.Api
{
    public class ServicioAdicionalApiService : BaseApiService<ServicioAdicionalDTO, CreateServicioAdicionalDTO, UpdateServicioAdicionalDTO, DeleteServicioAdicionalDTO>, IServicioAdicionalApiService
    {
        public ServicioAdicionalApiService(HttpClient httpClient, ILogger<ServicioAdicionalApiService> logger)
            : base(httpClient, logger)
        {
        }

        protected override string EntityName => "Servicio Adicional";
        protected override string BaseEndpoint => "ServicioAdicional";
    }
}

