using System.Net.Http.Json;
using SGHR.Application.DTOs.Clientes.Cliente;

namespace SGHR.Web.Infrastructure.HttpClients
{
    public class ClienteHttpClient
    {
        public HttpClient client = new HttpClient();
        private const string BaseApiAddress = "http://localhost:5066/api/";

        public ClienteHttpClient()
        {
            client.BaseAddress = new Uri(BaseApiAddress);
            client.Timeout = TimeSpan.FromSeconds(30);
        }

        public async Task<HttpResponseMessage> Index()
        {
            return await client.GetAsync("Cliente");
        }

        public async Task<HttpResponseMessage> Details(int id)
        {
            return await client.GetAsync($"Cliente/{id}");
        }

        public async Task<HttpResponseMessage> Create(ClienteCreateDTO dto)
        {
            return await client.PostAsJsonAsync("Cliente", dto);
        }

        public async Task<HttpResponseMessage> Edit(ClienteUpdateDTO dto)
        {
            return await client.PutAsJsonAsync("Cliente", dto);
        }

        public async Task<HttpResponseMessage> Delete(int id)
        {
            return await client.DeleteAsync($"Cliente/{id}");
        }
    }
}

