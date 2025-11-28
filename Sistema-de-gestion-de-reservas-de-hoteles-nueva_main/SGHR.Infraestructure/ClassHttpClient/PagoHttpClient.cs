using System.Net.Http.Json;
using SGHR.Infraestructure.Interfaces;

namespace SGHR.Infraestructure.ClassHttpClient
{
    public class PagoHttpClient : IHttpClientBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public PagoHttpClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<HttpResponseMessage> Index()
        {
            using (HttpClient client = _httpClientFactory.CreateClient("SGHRAPI"))
            {
                return await client.GetAsync("Pago");
            }
        }

        public async Task<HttpResponseMessage> Details(int id)
        {
            using (HttpClient client = _httpClientFactory.CreateClient("SGHRAPI"))
            {
                return await client.GetAsync($"Pago/{id}");
            }
        }

        public async Task<HttpResponseMessage> Create(object model)
        {
            using (HttpClient client = _httpClientFactory.CreateClient("SGHRAPI"))
            {
                return await client.PostAsJsonAsync("Pago", model);
            }
        }

        public async Task<HttpResponseMessage> Edit(object model)
        {
            using (HttpClient client = _httpClientFactory.CreateClient("SGHRAPI"))
            {
                return await client.PutAsJsonAsync("Pago", model);
            }
        }

        public async Task<HttpResponseMessage> Delete(int id)
        {
            using (HttpClient client = _httpClientFactory.CreateClient("SGHRAPI"))
            {
                return await client.DeleteAsync($"Pago/{id}");
            }
        }
    }
}

