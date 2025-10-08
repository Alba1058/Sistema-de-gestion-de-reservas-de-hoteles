using SGHR.Application.DTOs.Clientes.Cliente;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGHR.Application.Interfaces.Clientes
{
    public interface IClienteService
    {
        Task<ClienteDTO?> GetByIdAsync(int id);
        Task<IEnumerable<ClienteDTO>> GetAllAsync();
        Task<IEnumerable<ClienteDTO>> GetAllWithReservasAsync();
        Task<ClienteDTO> CreateAsync(ClienteCreateDTO dto);
        Task<bool> UpdateAsync(ClienteUpdateDTO dto);
        Task<bool> DeleteAsync(ClienteDeleteDTO dto);
    }
}