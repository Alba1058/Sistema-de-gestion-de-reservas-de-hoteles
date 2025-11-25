using Microsoft.Extensions.DependencyInjection;
using SGHR.Application.Interfaces.Configuration;
using SGHR.Application.Services.Configuration;
using SGHR.Persistence.Interfaces.Configuration;
using SGHR.Persistence.Repositories.Configuration;

namespace SGHR.IOC.Dependencias.Configuration
{
    public static class RolUsuarioDependences
    {
        public static IServiceCollection AddRolUsuarioDependences(this IServiceCollection services)
        {
            services.AddScoped<IRolUsuarioService, RolUsuarioService>();
            services.AddScoped<IRolUsuarioRepository, RolUsuarioRepository>();
           

            return services;
        }
    }
}
