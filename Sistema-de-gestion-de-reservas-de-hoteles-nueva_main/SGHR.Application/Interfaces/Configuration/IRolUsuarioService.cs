using SGHR.Application.DTOs.Configuration.RolUsuario;
using SGHR.Domain.Base;

namespace SGHR.Application.Interfaces.Configuration
{
    public interface IRolUsuarioService : IBaseService<CreateRolUsuarioDTO, UpdateRolUsuarioDTO, DeleteRolUsuarioDTO, RolUsuarioDTO>
    {
        Task<OperationResult<List<RolUsuarioDTO>>> GetRolesActivosAsync();
    }
}
