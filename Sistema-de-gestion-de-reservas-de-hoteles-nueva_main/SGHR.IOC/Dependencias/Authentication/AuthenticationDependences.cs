using Microsoft.Extensions.DependencyInjection;
using SGHR.Application.Interfaces;
using SGHR.Application.Interfaces.Authentication;
using SGHR.Application.Services;
using SGHR.Application.Services.Authentication;

namespace SGHR.IOC.Dependencias.Authentication
{
    public static class AuthenticationDependences
    {
        public static IServiceCollection AddAuthenticationDependences(this IServiceCollection services)
        {
            services.AddScoped<IAuthenticationServices, AuthenticationServices>();

            return services;
        }
    }
}
