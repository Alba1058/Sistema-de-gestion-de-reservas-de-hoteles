using SGHR.Domain.Entities.Usuarios;
using SGHR.Domain.Repository;

namespace SGHR.Persistence.Interfaces.Usuarios
{
    public interface IUsuarioRepository : IBaseRepository<Usuario>
    {
        Task<Usuario?> GetUsuarioByCorreoAsync(string correo);
    }
}
