using SGHR.Domain.Entities.Reservas;
using SGHR.Domain.Repository;

namespace SGHR.Persistence.Interfaces.Reservas
{
    public interface IServicioAdicionalRepository : IBaseRepository<ServicioAdicional>
    {
        Task<List<ServicioAdicional>> GetServiciosDisponiblesAsync();
    }
}
