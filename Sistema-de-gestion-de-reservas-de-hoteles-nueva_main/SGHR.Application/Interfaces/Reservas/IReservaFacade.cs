using SGHR.Application.DTOs.Clientes.Cliente;
using SGHR.Application.DTOs.Reservas.Habitacion;
using SGHR.Application.DTOs.Reservas.Reserva;
using SGHR.Domain.Base;

namespace SGHR.Application.Interfaces.Reservas
{
    public interface IReservaFacade
    {
        Task<OperationResult<ReservaFacadeData>> GetCreateReservaDataAsync();
        Task<OperationResult<ReservaFacadeData>> GetEditReservaDataAsync(int reservaId);
    }

    public class ReservaFacadeData
    {
        public List<ClienteDTO> Clientes { get; set; } = new();
        public List<HabitacionDTO> Habitaciones { get; set; } = new();
        public ReservaDTO? Reserva { get; set; }
    }
}

