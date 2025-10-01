using SGHR.Domain.Entities.Usuarios;
using SGHR.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGHR.Persistence.Interfaces.Usuarios
{
    public interface IUsuarioRepository : IBaseRepository<Usuario>
    {
        Task<Usuario?> GetUsuarioByCorreoAsync(string correo);
    }
}
