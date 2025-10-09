using SGHR.Application.Interfaces.Clientes;
using SGHR.Application.Interfaces.Configuration;
using SGHR.Application.Interfaces.Reservas;
using SGHR.Application.Interfaces.Usuarios;
using SGHR.Application.Services.Clientes;
using SGHR.Application.Services.Configuracion;
using SGHR.Application.Services.Reservas;
using SGHR.Application.Services.Usuarios;


namespace SGHR.Api.Extends
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register application services 

            // Clientes
            services.AddScoped<IClienteService, ClienteService>();

            // Configuration
            services.AddScoped<ICategoriaService, CategoriaService>();
            services.AddScoped<IPisoService, PisoService>();
            services.AddScoped<IRolUsuarioService, RolUsuarioService>();

            // Reservas
            services.AddScoped<IHabitacionService, HabitacionService>();
            services.AddScoped<IReservaService, ReservaService>();
            services.AddScoped<IServicioAdicionalService, ServicioAdicionalService>();
            services.AddScoped<ITarifaService, TarifaService>();
            services.AddScoped<IPagoService, PagoService>();

            // Usuarios
            services.AddScoped<IUsuarioService, UsuarioService>();





            return services;
        }
    }
}
