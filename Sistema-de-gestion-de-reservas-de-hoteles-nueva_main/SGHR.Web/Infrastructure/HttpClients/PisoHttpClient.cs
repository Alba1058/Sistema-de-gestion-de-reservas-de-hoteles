using System.Net.Http.Json;
using SGHR.Application.DTOs.Configuration.Piso;

namespace SGHR.Web.Infrastructure.HttpClients
{
    public class PisoHttpClient
    {
        public HttpClient client = new HttpClient();
        private const string BaseApiAddress = "http://localhost:5066/api/";

        public PisoHttpClient()
        {
            client.BaseAddress = new Uri(BaseApiAddress);
        }

        public async Task<HttpResponseMessage> Index()
        {
            return await client.GetAsync("Piso");
        }

        public async Task<HttpResponseMessage> Details(int id)
        {
            return await client.GetAsync($"Piso/{id}");
        }

        public async Task<HttpResponseMessage> Create(CreatePisoDTO dto)
        {
            return await client.PostAsJsonAsync("Piso", dto);
        }

        public async Task<HttpResponseMessage> Edit(UpdatePisoDTO dto)
        {
            return await client.PutAsJsonAsync("Piso", dto);
        }

        public async Task<HttpResponseMessage> Delete(int id)
        {
            return await client.DeleteAsync($"Piso/{id}");
        }
    }
}

