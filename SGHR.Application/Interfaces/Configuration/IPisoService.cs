using SGHR.Application.DTOs.Configuration.Piso;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGHR.Application.Interfaces.Configuration
{

    public interface IPisoService
    {
        Task<PisoDTO?> GetByIdAsync(int id);
        Task<IEnumerable<PisoDTO>> GetAllAsync();
        Task<PisoDTO> CreateAsync(CreatePisoDTO dto);
        Task<bool> UpdateAsync(UpdatePisoDTO dto);
        Task<bool> DeleteAsync(DeletePisoDTO dto);
    }
}