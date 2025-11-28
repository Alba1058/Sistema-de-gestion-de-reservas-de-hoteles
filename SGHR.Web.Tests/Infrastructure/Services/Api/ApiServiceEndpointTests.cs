using System.Net;
using SGHR.Web.Infrastructure.Services.Api.Services;
using SGHR.Web.Tests.TestUtilities;

namespace SGHR.Web.Tests.Infrastructure.Services.Api;

public class ApiServiceEndpointTests
{
    public static IEnumerable<object[]> ServiceEndpointData =>
        new List<object[]>
        {
            new object[] { typeof(ClienteApiService), "Cliente" },
            new object[] { typeof(UsuarioApiService), "Usuario" },
            new object[] { typeof(CategoriaApiService), "Categoria" },
            new object[] { typeof(PisoApiService), "Piso" },
            new object[] { typeof(RolUsuarioApiService), "RolUsuario" },
            new object[] { typeof(HabitacionApiService), "Habitacion" },
            new object[] { typeof(ReservaApiService), "Reserva" },
            new object[] { typeof(PagoApiService), "Pago" },
            new object[] { typeof(TarifaApiService), "Tarifa" },
            new object[] { typeof(ServicioAdicionalApiService), "ServicioAdicional" }
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
        Assert.Equal($"/{expectedEndpoint}", requestedPath);
    }
}

