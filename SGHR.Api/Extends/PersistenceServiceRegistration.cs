using SGHR.Persistence.Interfaces.Clientes;
using SGHR.Persistence.Interfaces.Configuracion;
using SGHR.Persistence.Interfaces.Reservas;
using SGHR.Persistence.Interfaces.Usuarios;
using SGHR.Persistence.Repositories.Clientes;
using SGHR.Persistence.Repositories.Configuration;
using SGHR.Persistence.Repositories.Reservas;
using SGHR.Persistence.Repositories.Usuarios;
using System.Runtime.CompilerServices;

namespace SGHR.Api.Extends
{
    public static class PersistenceServiceRegistration
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services)
        {
            // Register application services 

            // Clientes
            services.AddScoped<IClienteRepository, ClienteRepository>();

            // Configuracion
            services.AddScoped<ICategoriaRepository, CategoriaRepositoryADO>();
            services.AddScoped<IPisoRepository, PisoRepository>();
            services.AddScoped<IRolUsuarioRepository, RolUsuarioRepository>();

            // Reservas
            services.AddScoped<IHabitacionRepository, HabitacionRepository>();
            services.AddScoped<IReservaRepository, ReservaRepository>();
            services.AddScoped<IServicioAdicionalRepository, ServiciosAdicionalesRepository>();
            services.AddScoped<ITarifaRepository, TarifaRepositoryADO>();
            services.AddScoped<IPagoRepository, PagoRepositoryADO>();

            // Usuarios
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();



            return services;

        }
    }
}
