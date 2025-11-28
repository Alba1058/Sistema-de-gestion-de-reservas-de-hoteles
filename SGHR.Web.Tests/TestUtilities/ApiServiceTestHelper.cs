using System.Net;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Logging;
using Moq;

namespace SGHR.Web.Tests.TestUtilities;

internal static class ApiServiceTestHelper
{
    public static IHttpClientFactory CreateHttpClientFactory(Func<HttpRequestMessage, HttpResponseMessage> handlerFunc)
    {
        var messageHandler = new DelegatingHandlerStub(handlerFunc);
        var httpClient = new HttpClient(messageHandler)
        {
            BaseAddress = new Uri("http://localhost/api/")
        };

        var factoryMock = new Mock<IHttpClientFactory>();
        factoryMock
            .Setup(factory => factory.CreateClient("SGHRAPI"))
            .Returns(httpClient);

        return factoryMock.Object;
    }

    public static ILogger<T> CreateLogger<T>()
        where T : class
        => Mock.Of<ILogger<T>>();

    public static object CreateLogger(Type serviceType)
    {
        var loggerType = typeof(ILogger<>).MakeGenericType(serviceType);
        var mockType = typeof(Mock<>).MakeGenericType(loggerType);
        var mockInstance = Activator.CreateInstance(mockType);
        return mockType.GetProperty("Object")!.GetValue(mockInstance)!;
    }

    public static HttpResponseMessage CreateJsonResponse(HttpStatusCode statusCode, string jsonContent)
        => new(statusCode)
        {
            Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
        };

    private sealed class DelegatingHandlerStub : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _handlerFunc;

        public DelegatingHandlerStub(Func<HttpRequestMessage, HttpResponseMessage> handlerFunc)
        {
            _handlerFunc = handlerFunc ?? throw new ArgumentNullException(nameof(handlerFunc));
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromResult(_handlerFunc(request));
    }
}

