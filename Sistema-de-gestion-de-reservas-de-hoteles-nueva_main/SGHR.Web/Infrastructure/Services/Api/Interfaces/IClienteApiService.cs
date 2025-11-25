using SGHR.Application.DTOs.Clientes.Cliente;
using SGHR.Domain.Base;

namespace SGHR.Web.Infrastructure.Services.Api.Interfaces
{
    public interface IClienteApiService : IApiService<ClienteDTO, ClienteCreateDTO, ClienteUpdateDTO, ClienteDeleteDTO>
    {
    }
}

