using SGHR.Application.DTOs.Reservas.Habitacion;
using SGHR.Domain.Base;

namespace SGHR.Application.Interfaces.Reservas
{
    public interface IHabitacionService : IBaseService<CreateHabitacionDTO, UpdateHabitacionDTO, DeleteHabitacionDTO, HabitacionDTO>
    {
        Task<OperationResult<List<HabitacionDTO>>> GetHabitacionesDisponiblesAsync();
    }
}