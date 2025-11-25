using Microsoft.Extensions.DependencyInjection;
using SGHR.Application.Interfaces.Configuration;
using SGHR.Application.Services.Configuration;
using SGHR.Persistence.Interfaces.Configuration;
using SGHR.Persistence.Repositories.Configuration;

namespace SGHR.IOC.Dependencias.Configuration
{
    public static class PisoDependences
    {
        public static IServiceCollection AddPisoDependences(this IServiceCollection services)
        {
            services.AddScoped<IPisoService, PisoService>();
            services.AddScoped<IPisoRepository, PisoRepository>();


            return services;
        }
    }
}
