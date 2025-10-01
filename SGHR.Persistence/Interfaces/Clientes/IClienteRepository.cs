using SGHR.Domain.Entities.Clientes;
using SGHR.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGHR.Persistence.Interfaces.Clientes
{
    public interface IClienteRepository : IBaseRepository<Cliente>
    {
        Task<List<Cliente>> GetClientesConReservasAsync();
    }
}
