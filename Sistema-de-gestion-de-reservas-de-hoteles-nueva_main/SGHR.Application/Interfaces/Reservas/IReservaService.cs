using SGHR.Application.DTOs.Reservas.Reserva;
using SGHR.Domain.Base;

namespace SGHR.Application.Interfaces.Reservas
{
    public interface IReservaService : IBaseService<CreateReservaDTO, UpdateReservaDTO, DeleteReservaDTO, ReservaDTO>
    {
        Task<OperationResult<List<ReservaDTO>>> GetReservasPorFechaAsync(DateTime inicio, DateTime fin);
        Task<OperationResult<List<ReservaDTO>>> GetReservasPorClienteAsync(int clienteId);
        Task<OperationResult<bool>> CancelarReservaAsync(int reservaId);
    }
}