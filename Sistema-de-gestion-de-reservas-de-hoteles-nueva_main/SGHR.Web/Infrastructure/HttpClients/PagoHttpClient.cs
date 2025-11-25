using System.Net.Http.Json;
using SGHR.Application.DTOs.Reservas.Pago;

namespace SGHR.Web.Infrastructure.HttpClients
{
    public class PagoHttpClient
    {
        public HttpClient client = new HttpClient();
        private const string BaseApiAddress = "http://localhost:5066/api/";

        public PagoHttpClient()
        {
            client.BaseAddress = new Uri(BaseApiAddress);
        }

        public async Task<HttpResponseMessage> Index()
        {
            return await client.GetAsync("Pago");
        }

        public async Task<HttpResponseMessage> Details(int id)
        {
            return await client.GetAsync($"Pago/{id}");
        }

        public async Task<HttpResponseMessage> Create(CreatePagoDTO dto)
        {
            return await client.PostAsJsonAsync("Pago", dto);
        }

        public async Task<HttpResponseMessage> Edit(UpdatePagoDTO dto)
        {
            return await client.PutAsJsonAsync("Pago", dto);
        }

        public async Task<HttpResponseMessage> Delete(int id)
        {
            return await client.DeleteAsync($"Pago/{id}");
        }
    }
}

