using System.Net;
using SGHR.Web.Infrastructure.Services.Api.Services;
using SGHR.Web.Tests.TestUtilities;

namespace SGHR.Web.Tests.Infraestructure;

public class ApiServiceEndpointTests
{
    public static TheoryData<Type, string> ServiceEndpointData => new()
    {
        { typeof(ClienteApiService), "Cliente" },
        { typeof(UsuarioApiService), "Usuario" },
        { typeof(CategoriaApiService), "Categoria" },
        { typeof(PisoApiService), "Piso" },
        { typeof(RolUsuarioApiService), "RolUsuario" },
        { typeof(HabitacionApiService), "Habitacion" },
        { typeof(ReservaApiService), "Reserva" },
        { typeof(PagoApiService), "Pago" },
        { typeof(TarifaApiService), "Tarifa" },
        { typeof(ServicioAdicionalApiService), "ServicioAdicional" }
    };

    [Theory]
    [MemberData(nameof(ServiceEndpointData))]
    public async Task GetAllAsync_ShouldCallExpectedEndpoint(Type serviceType, string expectedEndpoint)
    {
        string? requestedPath = null;

        var factory = ApiServiceTestHelper.CreateHttpClientFactory(request =>
        {
            requestedPath = request.RequestUri?.AbsolutePath;
            return ApiServiceTestHelper.CreateJsonResponse(HttpStatusCode.OK, "{\"success\":true,\"message\":\"OK\",\"data\":[]}");
        });

        var logger = ApiServiceTestHelper.CreateLogger(serviceType);
        var serviceInstance = Activator.CreateInstance(serviceType, factory, logger)
            ?? throw new InvalidOperationException($"No fue posible crear el servicio {serviceType.Name}");

        dynamic service = serviceInstance;
        var result = await service.GetAllAsync();

        Assert.True(result.Success);
        Assert.Equal($"/api/{expectedEndpoint}", requestedPath);
    }
}

