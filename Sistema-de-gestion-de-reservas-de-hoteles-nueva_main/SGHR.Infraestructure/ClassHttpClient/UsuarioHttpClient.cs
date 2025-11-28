using System.Net.Http.Json;
using SGHR.Infraestructure.Interfaces;

namespace SGHR.Infraestructure.ClassHttpClient
{
    public class UsuarioHttpClient : IHttpClientBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public UsuarioHttpClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<HttpResponseMessage> Index()
        {
            using (HttpClient client = _httpClientFactory.CreateClient("SGHRAPI"))
            {
                return await client.GetAsync("Usuario");
            }
        }

        public async Task<HttpResponseMessage> Details(int id)
        {
            using (HttpClient client = _httpClientFactory.CreateClient("SGHRAPI"))
            {
                return await client.GetAsync($"Usuario/{id}");
            }
        }

        public async Task<HttpResponseMessage> Create(object model)
        {
            using (HttpClient client = _httpClientFactory.CreateClient("SGHRAPI"))
            {
                return await client.PostAsJsonAsync("Usuario", model);
            }
        }

        public async Task<HttpResponseMessage> Edit(object model)
        {
            using (HttpClient client = _httpClientFactory.CreateClient("SGHRAPI"))
            {
                return await client.PutAsJsonAsync("Usuario", model);
            }
        }

        public async Task<HttpResponseMessage> Delete(int id)
        {
            using (HttpClient client = _httpClientFactory.CreateClient("SGHRAPI"))
            {
                return await client.DeleteAsync($"Usuario/{id}");
            }
        }
    }
}

