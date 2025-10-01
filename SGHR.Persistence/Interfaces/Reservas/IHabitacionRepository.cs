using SGHR.Domain.Entities.Reservas;
using SGHR.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGHR.Persistence.Interfaces.Reservas
{
    public interface IHabitacionRepository : IBaseRepository<Habitacion>
    {
        Task<List<Habitacion>> GetHabitacionesDisponiblesAsync();
    }
}
