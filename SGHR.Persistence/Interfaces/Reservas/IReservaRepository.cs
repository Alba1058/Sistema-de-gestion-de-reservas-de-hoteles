using SGHR.Domain.Entities.Reservas;
using SGHR.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGHR.Persistence.Interfaces.Reservas
{
    public interface IReservaRepository : IBaseRepository<Reserva>
    {
        Task<List<Reserva>> GetReservasPorFechaAsync(DateTime inicio, DateTime fin);
        Task<List<Reserva>> GetReservasPorClienteAsync(int clienteId);
        Task<bool> CancelarReservaAsync(int reservaId);
    }
}
