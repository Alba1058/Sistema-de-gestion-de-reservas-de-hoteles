using SGHR.Application.DTOs.Usuarios.Usuario;
using SGHR.Domain.Base;

namespace SGHR.Application.Interfaces.Authentication
{
    public interface IAuthenticationServices
    {
        Task<OperationResult<UsuarioDTO>> LoginSesionAsync(string email, string password);
    }
}
