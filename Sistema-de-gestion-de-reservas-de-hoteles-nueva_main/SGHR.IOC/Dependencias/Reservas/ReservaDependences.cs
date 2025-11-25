using Microsoft.Extensions.DependencyInjection;
using SGHR.Application.Interfaces.Reservas;
using SGHR.Application.Services.Reservas;
using SGHR.Domain.Entities.Reservas;
using SGHR.Persistence.Interfaces.Reservas;
using SGHR.Persistence.Repositories.Reservas;

namespace SGHR.IOC.Dependencias.Reservas
{
    public static class ReservaDependences
    {
        public static IServiceCollection AddReservaDependences(this IServiceCollection services)
        {
            services.AddScoped<IReservaService, ReservaService>();
            services.AddScoped<IReservaRepository, ReservaRepository>();
            services.AddScoped<IReservaServicioRepository, ReservaServicioRepository>();

            return services;
        }
    }
}
