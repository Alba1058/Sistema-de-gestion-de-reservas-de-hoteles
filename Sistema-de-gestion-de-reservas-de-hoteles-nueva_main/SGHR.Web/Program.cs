using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using SGHR.IOC.Dependencias;
using SGHR.Persistence.Context;
using System.Net.Http.Headers;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using SGHR.Web.Infrastructure.Services.Api.Services;
using SGHR.Web.Infrastructure.Services.Api.Interfaces;
using SGHR.Web.Infrastructure.Services.Api.Facade;
using SGHR.Application.DTOs.Clientes.Cliente;
using SGHR.Application.DTOs.Usuarios.Usuario;
using SGHR.Application.DTOs.Configuration.Categoria;
using SGHR.Application.DTOs.Configuration.Piso;
using SGHR.Application.DTOs.Configuration.RolUsuario;
using SGHR.Application.DTOs.Reservas.Habitacion;
using SGHR.Application.DTOs.Reservas.Reserva;
using SGHR.Application.DTOs.Reservas.Pago;
using SGHR.Application.DTOs.Reservas.Tarifa;
using SGHR.Application.DTOs.Reservas.ServicioAdicional;
using SGHR.Web.Filters;

namespace SGHR.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews(options =>
            {
                options.Filters.Add<ApiExceptionFilterAttribute>();
            });

            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                var defaultCulture = "es-DO";
                var supportedCultures = new[] { new CultureInfo(defaultCulture) };

                options.DefaultRequestCulture = new RequestCulture(defaultCulture);
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;

                supportedCultures[0].NumberFormat.NumberDecimalSeparator = ".";
                supportedCultures[0].NumberFormat.CurrencyDecimalSeparator = ".";
            });


            builder.Services.Configure<RazorViewEngineOptions>(options =>
            {
                options.AreaViewLocationFormats.Add("/ApiConsumer/{2}/Views/{1}/{0}.cshtml");
                options.AreaViewLocationFormats.Add("/ApiConsumer/{2}/Views/Clientes/{1}/{0}.cshtml");
                options.AreaViewLocationFormats.Add("/ApiConsumer/{2}/Views/Reservas/{1}/{0}.cshtml");
                options.AreaViewLocationFormats.Add("/ApiConsumer/{2}/Views/Configuration/{1}/{0}.cshtml");
                options.AreaViewLocationFormats.Add("/ApiConsumer/{2}/Views/Usuarios/{1}/{0}.cshtml");

                options.ViewLocationFormats.Insert(0, "/ApiConsumer/Views/Clientes/{1}/{0}.cshtml");
                options.ViewLocationFormats.Insert(0, "/ApiConsumer/Views/Reservas/{1}/{0}.cshtml");
                options.ViewLocationFormats.Insert(0, "/ApiConsumer/Views/Configuration/{1}/{0}.cshtml");
                options.ViewLocationFormats.Insert(0, "/ApiConsumer/Views/Usuarios/{1}/{0}.cshtml");
                options.ViewLocationFormats.Insert(0, "/ApiConsumer/Views/{1}/{0}.cshtml");

                options.AreaViewLocationFormats.Insert(0, "/Areas/{2}/Views/Reservas/{1}/{0}.cshtml");
                options.AreaViewLocationFormats.Insert(0, "/Areas/{2}/Views/Clientes/{1}/{0}.cshtml");
                options.AreaViewLocationFormats.Insert(0, "/Areas/{2}/Views/Usuarios/{1}/{0}.cshtml");
                options.AreaViewLocationFormats.Insert(0, "/Areas/{2}/Views/Configuration/{1}/{0}.cshtml");
            });

            var apiBaseUrl = builder.Configuration["ApiSettings:SGHRAPI"] ?? "http://localhost:5066/api/";

            builder.Services.AddHttpClient("SGHRAPI", client =>
            {
                client.BaseAddress = new Uri(apiBaseUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            // Registrar Facades (Application Layer)
            builder.Services.AddScoped<SGHR.Application.Interfaces.Reservas.IReservaFacade, SGHR.Application.Facades_Classes.Reservas.ReservaFacade>();

            // Registrar Facades (Web Layer - adapta a ViewModels)
            builder.Services.AddScoped<IReservaApiFacade, ReservaApiFacade>();

            // Registrar servicios usando IHttpClientFactory
            builder.Services.AddScoped<IClienteApiService, ClienteApiService>();
            builder.Services.AddScoped<IUsuarioApiService, UsuarioApiService>();
            builder.Services.AddScoped<ICategoriaApiService, CategoriaApiService>();
            builder.Services.AddScoped<IPisoApiService, PisoApiService>();
            builder.Services.AddScoped<IRolUsuarioApiService, RolUsuarioApiService>();
            builder.Services.AddScoped<IHabitacionApiService, HabitacionApiService>();
            builder.Services.AddScoped<IReservaApiService, ReservaApiService>();
            builder.Services.AddScoped<IPagoApiService, PagoApiService>();
            builder.Services.AddScoped<ITarifaApiService, TarifaApiService>();
            builder.Services.AddScoped<IServicioAdicionalApiService, ServicioAdicionalApiService>();

            var connectionString = builder.Configuration.GetConnectionString("SghrConnString");
            builder.Services.AddDbContext<SGHRContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddDependences();
            builder.Services.AddSession();
            builder.Services.AddHttpContextAccessor();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            else
            {
                app.UseHttpsRedirection();
            }

            // Headers de seguridad HTTP
            app.Use(async (context, next) =>
            {
                context.Response.Headers["X-Content-Type-Options"] = "nosniff";
                context.Response.Headers["X-Frame-Options"] = "DENY";
                context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
                context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
                context.Response.Headers["Permissions-Policy"] = "geolocation=(), microphone=(), camera=()";
                
                // Content Security Policy 
                context.Response.Headers["Content-Security-Policy"] =
                    "default-src 'self' https://cdn.jsdelivr.net; script-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net; style-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net; img-src 'self' data: https://cdn.jsdelivr.net; connect-src 'self' https://cdn.jsdelivr.net ws://localhost:* wss://localhost:* http://localhost:*;";
                
                await next();
            });

            app.UseStaticFiles();

            app.UseRouting();

            app.UseRequestLocalization();

            app.UseAuthorization();
            app.UseSession();

            app.MapControllerRoute(
                name: "areaDefault",
                pattern: "{area:exists}/{controller}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");


            app.Run();
        }
    }
}
