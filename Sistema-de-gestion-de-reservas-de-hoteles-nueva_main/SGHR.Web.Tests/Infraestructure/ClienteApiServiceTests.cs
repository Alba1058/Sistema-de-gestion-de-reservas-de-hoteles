using System.Net;
using System.Net.Http;
using SGHR.Application.DTOs.Clientes.Cliente;
using SGHR.Web.Infrastructure.Services.Api.Services;
using SGHR.Web.Tests.TestUtilities;

namespace SGHR.Web.Tests.Infraestructure;

public class ClienteApiServiceTests
{
    private const string SuccessListResponse = "{\"success\":true,\"message\":\"OK\",\"data\":[]}";
    private const string SuccessObjectResponse = "{\"success\":true,\"message\":\"OK\",\"data\":{}}";

    [Fact]
    public async Task GetAllAsync_ShouldReturnSuccess_WhenApiRespondsOk()
    {
        string? requestedPath = null;

        var service = CreateService(request =>
        {
            requestedPath = request.RequestUri?.AbsolutePath;
            return ApiServiceTestHelper.CreateJsonResponse(HttpStatusCode.OK, SuccessListResponse);
        });

        var result = await service.GetAllAsync();

        Assert.True(result.Success);
        Assert.Equal("/api/Cliente", requestedPath);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnFail_WhenApiReturnsError()
    {
        var service = CreateService(_ =>
            ApiServiceTestHelper.CreateJsonResponse(HttpStatusCode.InternalServerError, "Error interno"));

        var result = await service.GetAllAsync();

        Assert.False(result.Success);
        Assert.Contains("Error al obtener Cliente:", result.Message);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnFail_WhenIdIsInvalid()
    {
        var service = CreateService(_ =>
            ApiServiceTestHelper.CreateJsonResponse(HttpStatusCode.OK, SuccessObjectResponse));

        var result = await service.GetByIdAsync(0);

        Assert.False(result.Success);
        Assert.Contains("El ID de Cliente no es válido", result.Message);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnFail_WhenDtoIsNull()
    {
        var service = CreateService(_ =>
            ApiServiceTestHelper.CreateJsonResponse(HttpStatusCode.OK, SuccessObjectResponse));

        var result = await service.CreateAsync(null!);

        Assert.False(result.Success);
        Assert.Contains("El objeto Cliente no puede ser nulo", result.Message);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnSuccess_WhenApiRespondsOk()
    {
        string? requestedPath = null;

        var service = CreateService(request =>
        {
            requestedPath = request.RequestUri?.AbsolutePath;
            return ApiServiceTestHelper.CreateJsonResponse(HttpStatusCode.OK, SuccessObjectResponse);
        });

        var result = await service.CreateAsync(new ClienteCreateDTO());

        Assert.True(result.Success);
        Assert.Equal("/api/Cliente", requestedPath);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFail_WhenApiReturnsError()
    {
        var service = CreateService(_ =>
            ApiServiceTestHelper.CreateJsonResponse(HttpStatusCode.BadRequest, "Error"));

        var result = await service.DeleteAsync(10);

        Assert.False(result.Success);
        Assert.Contains("Error al eliminar Cliente", result.Message);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnSuccess_WhenApiRespondsOk()
    {
        var service = CreateService(_ =>
            ApiServiceTestHelper.CreateJsonResponse(HttpStatusCode.OK, "{\"success\":true,\"message\":\"Eliminado\",\"data\":true}"));

        var result = await service.DeleteAsync(5);

        Assert.True(result.Success);
        Assert.True(result.Data);
    }

    private static ClienteApiService CreateService(Func<HttpRequestMessage, HttpResponseMessage> handlerFunc)
    {
        var factory = ApiServiceTestHelper.CreateHttpClientFactory(handlerFunc);
        var logger = ApiServiceTestHelper.CreateLogger<ClienteApiService>();
        return new ClienteApiService(factory, logger);
    }
}

