using SGHR.Domain.Entities.Reservas;
using SGHR.Domain.Repository;

namespace SGHR.Persistence.Interfaces.Reservas
{
    public interface IHabitacionRepository : IBaseRepository<Habitacion>
    {
        Task<List<Habitacion>> GetHabitacionesDisponiblesAsync();
    }
}
