using Microsoft.Extensions.Logging;
using SGHR.Application.DTOs.Reservas.Habitacion;
using SGHR.Domain.Base;
using SGHR.Web.Infrastructure.Services.Api.Base;
using SGHR.Web.Infrastructure.Services.Api.Interfaces;

namespace SGHR.Web.Infrastructure.Services.Api.Services
{
    public class HabitacionApiService : BaseApiService<HabitacionDTO, CreateHabitacionDTO, UpdateHabitacionDTO, DeleteHabitacionDTO>, IHabitacionApiService
    {
        public HabitacionApiService(IHttpClientFactory httpClientFactory, ILogger<HabitacionApiService> logger)
            : base(httpClientFactory, logger)
        {
        }

        protected override string EntityName => "Habitación";
        protected override string BaseEndpoint => "Habitacion";
    }
}

