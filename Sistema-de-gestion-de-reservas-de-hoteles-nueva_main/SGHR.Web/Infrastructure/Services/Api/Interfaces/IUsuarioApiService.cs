using SGHR.Application.DTOs.Usuarios.Usuario;
using SGHR.Domain.Base;

namespace SGHR.Web.Infrastructure.Services.Api.Interfaces
{
    public interface IUsuarioApiService : IApiService<UsuarioDTO, UsuarioCreateDTO, UsuarioUpdateDTO, UsuarioDeleteDTO>
    {
    }
}

