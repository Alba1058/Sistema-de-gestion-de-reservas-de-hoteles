using Microsoft.Extensions.DependencyInjection;
using SGHR.Application.Interfaces.Reservas;
using SGHR.Application.Services.Reservas;
using SGHR.Domain.Entities.Reservas;
using SGHR.Persistence.Interfaces.Reservas;
using SGHR.Persistence.Repositories.Reservas;

namespace SGHR.IOC.Dependencias.Reservas
{
    public static class PagoDependences
    {
        public static IServiceCollection AddPagoDependences(this IServiceCollection services)
        {
            services.AddScoped<IPagoService, PagoService>();
            services.AddScoped<IPagoRepository, PagoRepositoryAdo>();


            return services;
        }
    }
}
