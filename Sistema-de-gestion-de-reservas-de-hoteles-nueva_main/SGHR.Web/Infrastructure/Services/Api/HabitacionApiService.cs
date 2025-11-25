using Microsoft.Extensions.Logging;
using SGHR.Application.DTOs.Reservas.Habitacion;
using SGHR.Domain.Base;
using SGHR.Web.Infrastructure.Services.Api.Base;
using SGHR.Web.Infrastructure.Services.Api.Interfaces;

namespace SGHR.Web.Infrastructure.Services.Api
{
    public class HabitacionApiService : BaseApiService<HabitacionDTO, CreateHabitacionDTO, UpdateHabitacionDTO, DeleteHabitacionDTO>, IHabitacionApiService
    {
        public HabitacionApiService(HttpClient httpClient, ILogger<HabitacionApiService> logger)
            : base(httpClient, logger)
        {
        }

        protected override string EntityName => "Habitación";
        protected override string BaseEndpoint => "Habitacion";
    }
}

