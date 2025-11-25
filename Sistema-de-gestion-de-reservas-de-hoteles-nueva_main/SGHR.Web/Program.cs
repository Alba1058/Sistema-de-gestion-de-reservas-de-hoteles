using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using SGHR.IOC.Dependencias;
using SGHR.Persistence.Context;
using System.Net.Http.Headers;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using SGHR.Web.Infrastructure.Services.Api;
using SGHR.Web.Infrastructure.Services.Api.Interfaces;

namespace SGHR.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

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
            });

            var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5066/api/";

            builder.Services.AddHttpClient<IClienteApiService, ClienteApiService>(client =>
            {
                client.BaseAddress = new Uri(apiBaseUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            builder.Services.AddHttpClient<IUsuarioApiService, UsuarioApiService>(client =>
            {
                client.BaseAddress = new Uri(apiBaseUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            builder.Services.AddHttpClient<ICategoriaApiService, CategoriaApiService>(client =>
            {
                client.BaseAddress = new Uri(apiBaseUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });

            builder.Services.AddHttpClient<IPisoApiService, PisoApiService>(client =>
            {
                client.BaseAddress = new Uri(apiBaseUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });

            builder.Services.AddHttpClient<IRolUsuarioApiService, RolUsuarioApiService>(client =>
            {
                client.BaseAddress = new Uri(apiBaseUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });

            builder.Services.AddHttpClient<IHabitacionApiService, HabitacionApiService>(client =>
            {
                client.BaseAddress = new Uri(apiBaseUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });

            builder.Services.AddHttpClient<IReservaApiService, ReservaApiService>(client =>
            {
                client.BaseAddress = new Uri(apiBaseUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });

            builder.Services.AddHttpClient<IPagoApiService, PagoApiService>(client =>
            {
                client.BaseAddress = new Uri(apiBaseUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });

            builder.Services.AddHttpClient<ITarifaApiService, TarifaApiService>(client =>
            {
                client.BaseAddress = new Uri(apiBaseUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });

            builder.Services.AddHttpClient<IServicioAdicionalApiService, ServicioAdicionalApiService>(client =>
            {
                client.BaseAddress = new Uri(apiBaseUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });

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
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseRequestLocalization();

            app.UseAuthorization();
            app.UseSession();

            // La ruta por defecto debe ir antes de las áreas para evitar conflictos de routing
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");


            app.Run();
        }
    }
}
