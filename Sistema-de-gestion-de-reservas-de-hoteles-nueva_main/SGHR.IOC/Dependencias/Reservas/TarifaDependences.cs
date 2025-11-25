using Microsoft.Extensions.DependencyInjection;
using SGHR.Application.Interfaces.Reservas;
using SGHR.Application.Services.Reservas;
using SGHR.Domain.Entities.Reservas;
using SGHR.Persistence.Interfaces.Reservas;
using SGHR.Persistence.Repositories.Reservas;

namespace SGHR.IOC.Dependencias.Reservas
{
    public static class TarifaDependences
    {
        public static IServiceCollection AddTarifaDependences(this IServiceCollection services)
        {
            services.AddScoped<ITarifaService, TarifaService>();
            services.AddScoped<ITarifaRepository, TarifaRepositoryAdo>();

            return services;
        }
    }
}