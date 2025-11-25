using SGHR.Application.DTOs.Clientes.Cliente;
using SGHR.Domain.Base;

namespace SGHR.Application.Interfaces.Clientes
{
    public interface IClienteService : IBaseService<ClienteCreateDTO, ClienteUpdateDTO, ClienteDeleteDTO, ClienteDTO>
    {
        Task<OperationResult<List<ClienteDTO>>> GetClientesConReservasAsync();
    }
}