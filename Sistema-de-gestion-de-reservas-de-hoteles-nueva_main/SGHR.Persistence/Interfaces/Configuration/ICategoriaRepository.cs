using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration;
using SGHR.Domain.Repository;

namespace SGHR.Persistence.Interfaces.Configuration
{
    public interface ICategoriaRepository : IBaseRepository<Categoria>
    {
        Task<List<Categoria>> GetCategoriasActivas();
    }
}
