using System.Net.Http.Json;
using SGHR.Application.DTOs.Usuarios.Usuario;

namespace SGHR.Web.Infrastructure.HttpClients
{
    public class UsuarioHttpClient
    {
        public HttpClient client = new HttpClient();
        private const string BaseApiAddress = "http://localhost:5066/api/";

        public UsuarioHttpClient()
        {
            client.BaseAddress = new Uri(BaseApiAddress);
            client.Timeout = TimeSpan.FromSeconds(30);
        }

        public async Task<HttpResponseMessage> Index()
        {
            return await client.GetAsync("Usuario");
        }

        public async Task<HttpResponseMessage> Details(int id)
        {
            return await client.GetAsync($"Usuario/{id}");
        }

        public async Task<HttpResponseMessage> Create(UsuarioCreateDTO dto)
        {
            return await client.PostAsJsonAsync("Usuario", dto);
        }

        public async Task<HttpResponseMessage> Edit(UsuarioUpdateDTO dto)
        {
            return await client.PutAsJsonAsync("Usuario", dto);
        }

        public async Task<HttpResponseMessage> Delete(int id)
        {
            return await client.DeleteAsync($"Usuario/{id}");
        }
    }
}

