using System.Net.Http.Json;
using SGHR.Application.DTOs.Reservas.Reserva;

namespace SGHR.Web.Infrastructure.HttpClients
{
    public class ReservaHttpClient
    {
        public HttpClient client = new HttpClient();
        private const string BaseApiAddress = "http://localhost:5066/api/";

        public ReservaHttpClient()
        {
            client.BaseAddress = new Uri(BaseApiAddress);
        }

        public async Task<HttpResponseMessage> Index()
        {
            return await client.GetAsync("Reserva");
        }

        public async Task<HttpResponseMessage> Details(int id)
        {
            return await client.GetAsync($"Reserva/{id}");
        }

        public async Task<HttpResponseMessage> Create(CreateReservaDTO dto)
        {
            return await client.PostAsJsonAsync("Reserva", dto);
        }

        public async Task<HttpResponseMessage> Edit(UpdateReservaDTO dto)
        {
            return await client.PutAsJsonAsync("Reserva", dto);
        }

        public async Task<HttpResponseMessage> Delete(int id)
        {
            return await client.DeleteAsync($"Reserva/{id}");
        }
    }
}

