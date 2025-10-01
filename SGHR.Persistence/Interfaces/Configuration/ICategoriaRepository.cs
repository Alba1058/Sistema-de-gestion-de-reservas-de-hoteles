using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration;
using SGHR.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGHR.Persistence.Interfaces.Configuracion
{
    public interface ICategoriaRepository : IBaseRepository<Categoria>
    {
        Task<OperationResult> GetCategoriasActivas();
    }
}
