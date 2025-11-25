using Microsoft.Extensions.DependencyInjection;
using SGHR.Application.Interfaces.Reservas;
using SGHR.Application.Services.Reservas;
using SGHR.Persistence.Interfaces.Reservas;
using SGHR.Persistence.Repositories.Reservas;

namespace SGHR.IOC.Dependencias.Reservas
{
    public static class HabitacionDependences
    {
        public static IServiceCollection AddHabitacionDependences(this IServiceCollection services)
        {
            services.AddScoped<IHabitacionService, HabitacionService>();
            services.AddScoped<IHabitacionRepository, HabitacionRepository>();

            return services;
        }
    }
}