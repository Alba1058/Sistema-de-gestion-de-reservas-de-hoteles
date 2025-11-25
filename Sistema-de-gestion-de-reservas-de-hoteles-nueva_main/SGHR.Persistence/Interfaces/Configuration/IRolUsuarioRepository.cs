using SGHR.Domain.Entities.Configuration;
using SGHR.Domain.Repository;

namespace SGHR.Persistence.Interfaces.Configuration
{
    public interface IRolUsuarioRepository : IBaseRepository<RolUsuario>
    {
        Task<List<RolUsuario>> GetRolesActivosAsync();
    }
}
