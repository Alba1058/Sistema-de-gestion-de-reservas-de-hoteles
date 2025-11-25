using System.Net.Http.Json;
using SGHR.Application.DTOs.Configuration.RolUsuario;

namespace SGHR.Web.Infrastructure.HttpClients
{
    public class RolUsuarioHttpClient
    {
        public HttpClient client = new HttpClient();
        private const string BaseApiAddress = "http://localhost:5066/api/";

        public RolUsuarioHttpClient()
        {
            client.BaseAddress = new Uri(BaseApiAddress);
        }

        public async Task<HttpResponseMessage> Index()
        {
            return await client.GetAsync("RolUsuario");
        }

        public async Task<HttpResponseMessage> Details(int id)
        {
            return await client.GetAsync($"RolUsuario/{id}");
        }

        public async Task<HttpResponseMessage> Create(CreateRolUsuarioDTO dto)
        {
            return await client.PostAsJsonAsync("RolUsuario", dto);
        }

        public async Task<HttpResponseMessage> Edit(UpdateRolUsuarioDTO dto)
        {
            return await client.PutAsJsonAsync("RolUsuario", dto);
        }

        public async Task<HttpResponseMessage> Delete(int id)
        {
            return await client.DeleteAsync($"RolUsuario/{id}");
        }
    }
}

