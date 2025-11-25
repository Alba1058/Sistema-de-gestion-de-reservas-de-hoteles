using System.Net.Http.Json;
using SGHR.Application.DTOs.Configuration.Categoria;

namespace SGHR.Web.Infrastructure.HttpClients
{
    public class CategoriaHttpClient
    {
        public HttpClient client = new HttpClient();
        private const string BaseApiAddress = "http://localhost:5066/api/";

        public CategoriaHttpClient()
        {
            client.BaseAddress = new Uri(BaseApiAddress);
        }

        public async Task<HttpResponseMessage> Index()
        {
            return await client.GetAsync("Categoria");
        }

        public async Task<HttpResponseMessage> Details(int id)
        {
            return await client.GetAsync($"Categoria/{id}");
        }

        public async Task<HttpResponseMessage> Create(CreateCategoriaDTO dto)
        {
            return await client.PostAsJsonAsync("Categoria", dto);
        }

        public async Task<HttpResponseMessage> Edit(UpdateCategoriaDTO dto)
        {
            return await client.PutAsJsonAsync("Categoria", dto);
        }

        public async Task<HttpResponseMessage> Delete(int id)
        {
            return await client.DeleteAsync($"Categoria/{id}");
        }
    }
}

