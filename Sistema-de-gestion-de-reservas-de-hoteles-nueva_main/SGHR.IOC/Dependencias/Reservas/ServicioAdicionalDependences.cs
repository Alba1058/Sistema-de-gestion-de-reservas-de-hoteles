using Microsoft.Extensions.DependencyInjection;
using SGHR.Application.Interfaces.Reservas;
using SGHR.Application.Services.Reservas;
using SGHR.Domain.Entities.Reservas;
using SGHR.Persistence.Interfaces.Reservas;
using SGHR.Persistence.Repositories.Reservas;

namespace SGHR.IOC.Dependencias.Reservas
{
    public static class ServicioAdicionalDependences
    {
        public static IServiceCollection AddServicioAdicionalDependences(this IServiceCollection services)
        {
            services.AddScoped<IServicioAdicionalService, ServicioAdicionalService>();
            services.AddScoped<IServicioAdicionalRepository, ServicioAdicionalRepository>();


            return services;
        }
    }
}
