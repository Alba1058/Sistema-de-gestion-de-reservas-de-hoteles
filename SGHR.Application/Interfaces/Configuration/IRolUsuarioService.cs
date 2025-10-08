using SGHR.Application.DTOs.Configuration.RolUsuario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGHR.Application.Interfaces.Configuration
{
    public interface IRolUsuarioService
    {
        Task<RolUsuarioDTO?> GetByIdAsync(int id);
        Task<IEnumerable<RolUsuarioDTO>> GetAllAsync();
        Task<RolUsuarioDTO> CreateAsync(CreateRolUsuarioDTO dto);
        Task<bool> UpdateAsync(UpdateRolUsuarioDTO dto);
        Task<bool> DeleteAsync(DeleteRolUsuarioDTO dto);
    }
}