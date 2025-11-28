using SGHR.Domain.Base;
using SGHR.Domain.Entities.Reservas;
using SGHR.Domain.Repository;

namespace SGHR.Persistence.Interfaces.Reservas
{
    public interface IReservaRepository : IBaseRepository<Reserva>
    {
        Task<OperationResult<List<Reserva>>> GetReservasPorFechaAsync(DateTime inicio, DateTime fin);
        Task<OperationResult<List<Reserva>>> GetReservasPorClienteAsync(int clienteId);
    }
}
