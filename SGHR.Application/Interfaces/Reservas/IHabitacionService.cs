using SGHR.Application.DTOs.Reservas.Habitacion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGHR.Application.Interfaces.Reservas
{
    public interface IHabitacionService
    {
        Task<HabitacionDTO?> GetByIdAsync(int id);
        Task<IEnumerable<HabitacionDTO>> GetAllAsync();
        Task<IEnumerable<HabitacionDTO>> GetDisponiblesAsync();
        Task<HabitacionDTO> CreateAsync(CreateHabitacionDTO dto);
        Task<bool> UpdateAsync(UpdateHabitacionDTO dto);
        Task<bool> DeleteAsync(DeleteHabitacionDTO dto);
    }
}
