using Microsoft.Extensions.DependencyInjection;
using SGHR.Application.Interfaces.Usuarios;
using SGHR.Application.Services.Usuarios;
using SGHR.Domain.Entities.Usuarios;
using SGHR.Persistence.Interfaces.Usuarios;
using SGHR.Persistence.Repositories.Usuarios;

namespace SGHR.IOC.Dependencias.Usuarios
{
    public static class UsuarioDependences
    {
        public static IServiceCollection AddUsuarioDependences(this IServiceCollection services)
        {
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();


            return services;
        }
    }
}