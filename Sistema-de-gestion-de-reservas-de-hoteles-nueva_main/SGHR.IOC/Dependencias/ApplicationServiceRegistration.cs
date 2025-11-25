using Microsoft.Extensions.DependencyInjection;
using SGHR.IOC.Dependencias.Authentication;
using SGHR.IOC.Dependencias.Clientes;
using SGHR.IOC.Dependencias.Configuration;
using SGHR.IOC.Dependencias.Reservas;
using SGHR.IOC.Dependencias.Usuarios;

namespace SGHR.IOC.Dependencias
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddDependences(this IServiceCollection services)
        {
            // Clientes
            services = services.AddClienteDependences();

            // Configuration
            services = services.AddCategoriaDependences();
            services = services.AddPisoDependences();
            services = services.AddRolUsuarioDependences();

            // Reservas
            services = services.AddHabitacionDependences();
            services = services.AddPagoDependences();
            services = services.AddReservaDependences();
            services = services.AddServicioAdicionalDependences();
            services = services.AddTarifaDependences();

            // Usuarios
            services = services.AddUsuarioDependences();

            services = services.AddAuthenticationDependences();


            return services;
        }
    }
}