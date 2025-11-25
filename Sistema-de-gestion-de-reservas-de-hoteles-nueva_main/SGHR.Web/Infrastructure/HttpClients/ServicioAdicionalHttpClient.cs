using System.Net.Http.Json;
using SGHR.Application.DTOs.Reservas.ServicioAdicional;

namespace SGHR.Web.Infrastructure.HttpClients
{
    public class ServicioAdicionalHttpClient
    {
        public HttpClient client = new HttpClient();
        private const string BaseApiAddress = "http://localhost:5066/api/";

        public ServicioAdicionalHttpClient()
        {
            client.BaseAddress = new Uri(BaseApiAddress);
        }

        public async Task<HttpResponseMessage> Index()
        {
            return await client.GetAsync("ServicioAdicional");
        }

        public async Task<HttpResponseMessage> Details(int id)
        {
            return await client.GetAsync($"ServicioAdicional/{id}");
        }

        public async Task<HttpResponseMessage> Create(CreateServicioAdicionalDTO dto)
        {
            return await client.PostAsJsonAsync("ServicioAdicional", dto);
        }

        public async Task<HttpResponseMessage> Edit(UpdateServicioAdicionalDTO dto)
        {
            return await client.PutAsJsonAsync("ServicioAdicional", dto);
        }

        public async Task<HttpResponseMessage> Delete(int id)
        {
            return await client.DeleteAsync($"ServicioAdicional/{id}");
        }
    }
}

