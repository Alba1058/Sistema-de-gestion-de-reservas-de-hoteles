using System.Net.Http.Json;
using SGHR.Infraestructure.Interfaces;

namespace SGHR.Infraestructure.ClassHttpClient
{
    public class HabitacionHttpClient : IHttpClientBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HabitacionHttpClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<HttpResponseMessage> Index()
        {
            using (HttpClient client = _httpClientFactory.CreateClient("SGHRAPI"))
            {
                return await client.GetAsync("Habitacion");
            }
        }

        public async Task<HttpResponseMessage> Details(int id)
        {
            using (HttpClient client = _httpClientFactory.CreateClient("SGHRAPI"))
            {
                return await client.GetAsync($"Habitacion/{id}");
            }
        }

        public async Task<HttpResponseMessage> Create(object model)
        {
            using (HttpClient client = _httpClientFactory.CreateClient("SGHRAPI"))
            {
                return await client.PostAsJsonAsync("Habitacion", model);
            }
        }

        public async Task<HttpResponseMessage> Edit(object model)
        {
            using (HttpClient client = _httpClientFactory.CreateClient("SGHRAPI"))
            {
                return await client.PutAsJsonAsync("Habitacion", model);
            }
        }

        public async Task<HttpResponseMessage> Delete(int id)
        {
            using (HttpClient client = _httpClientFactory.CreateClient("SGHRAPI"))
            {
                return await client.DeleteAsync($"Habitacion/{id}");
            }
        }
    }
}

