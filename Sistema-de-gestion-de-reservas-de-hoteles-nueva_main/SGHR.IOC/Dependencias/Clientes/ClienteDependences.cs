using Microsoft.Extensions.DependencyInjection;
using SGHR.Application.Interfaces.Clientes;
using SGHR.Application.Services.Clientes;
using SGHR.Persistence.Interfaces.Clientes;
using SGHR.Persistence.Repositories.Clientes;

namespace SGHR.IOC.Dependencias.Clientes
{
    public static class ClienteDependences
    {
        public static IServiceCollection AddClienteDependences(this IServiceCollection services)
        {
            services.AddScoped<IClienteService, ClienteService>();
            services.AddScoped<IClienteRepository, ClienteRepository>();



            return services;
        }
    }
}