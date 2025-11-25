using System.Net.Http.Json;
using SGHR.Application.DTOs.Reservas.Tarifa;

namespace SGHR.Web.Infrastructure.HttpClients
{
    public class TarifaHttpClient
    {
        public HttpClient client = new HttpClient();
        private const string BaseApiAddress = "http://localhost:5066/api/";

        public TarifaHttpClient()
        {
            client.BaseAddress = new Uri(BaseApiAddress);
        }

        public async Task<HttpResponseMessage> Index()
        {
            return await client.GetAsync("Tarifa");
        }

        public async Task<HttpResponseMessage> Details(int id)
        {
            return await client.GetAsync($"Tarifa/{id}");
        }

        public async Task<HttpResponseMessage> Create(CreateTarifaDTO dto)
        {
            return await client.PostAsJsonAsync("Tarifa", dto);
        }

        public async Task<HttpResponseMessage> Edit(UpdateTarifaDTO dto)
        {
            return await client.PutAsJsonAsync("Tarifa", dto);
        }

        public async Task<HttpResponseMessage> Delete(int id)
        {
            return await client.DeleteAsync($"Tarifa/{id}");
        }
    }
}

