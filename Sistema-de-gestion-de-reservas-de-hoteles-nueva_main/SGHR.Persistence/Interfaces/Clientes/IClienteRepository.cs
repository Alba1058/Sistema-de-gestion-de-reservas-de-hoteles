using SGHR.Domain.Entities.Clientes;
using SGHR.Domain.Repository;

namespace SGHR.Persistence.Interfaces.Clientes
{
    public interface IClienteRepository : IBaseRepository<Cliente>
    {
        Task<List<Cliente>> GetClientesConReservasAsync();
        Task<Cliente?> GetClienteByIdentificacionAsync(string identificacion);
    }
}
