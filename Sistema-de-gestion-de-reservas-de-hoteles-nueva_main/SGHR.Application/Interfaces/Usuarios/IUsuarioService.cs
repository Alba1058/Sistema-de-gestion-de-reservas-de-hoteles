using SGHR.Application.DTOs.Usuarios.Usuario;
using SGHR.Domain.Base;

namespace SGHR.Application.Interfaces.Usuarios
{
    public interface IUsuarioService : IBaseService<UsuarioCreateDTO, UsuarioUpdateDTO, UsuarioDeleteDTO, UsuarioDTO>
    {
        Task<OperationResult<UsuarioDTO>> GetByEmailAsync(string email);
    }
}