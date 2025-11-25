using System.Net.Http.Json;
using SGHR.Application.DTOs.Reservas.Habitacion;

namespace SGHR.Web.Infrastructure.HttpClients
{
    public class HabitacionHttpClient
    {
        public HttpClient client = new HttpClient();
        private const string BaseApiAddress = "http://localhost:5066/api/";

        public HabitacionHttpClient()
        {
            client.BaseAddress = new Uri(BaseApiAddress);
        }

        public async Task<HttpResponseMessage> Index()
        {
            return await client.GetAsync("Habitacion");
        }

        public async Task<HttpResponseMessage> Details(int id)
        {
            return await client.GetAsync($"Habitacion/{id}");
        }

        public async Task<HttpResponseMessage> Create(CreateHabitacionDTO dto)
        {
            return await client.PostAsJsonAsync("Habitacion", dto);
        }

        public async Task<HttpResponseMessage> Edit(UpdateHabitacionDTO dto)
        {
            return await client.PutAsJsonAsync("Habitacion", dto);
        }

        public async Task<HttpResponseMessage> Delete(int id)
        {
            return await client.DeleteAsync($"Habitacion/{id}");
        }
    }
}

