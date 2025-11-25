using Microsoft.Extensions.DependencyInjection;
using SGHR.Application.Interfaces.Configuration;
using SGHR.Application.Services.Configuration;
using SGHR.Domain.Entities.Configuration;
using SGHR.Persistence.Interfaces.Configuration;
using SGHR.Persistence.Repositories.Configuration;

namespace SGHR.IOC.Dependencias.Configuration
{
    public static class CategoriaDependences
    {
        public static IServiceCollection AddCategoriaDependences(this IServiceCollection services)
        {
            services.AddScoped<ICategoriaService, CategoriaService>();
            services.AddScoped<ICategoriaRepository, CategoriaRepository>();

            return services;
        }
    }
}
