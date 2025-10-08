using SGHR.Application.DTOs.Usuarios.Usuario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGHR.Application.Interfaces.Usuarios
{
    public interface IUsuarioService
    {
        Task<UsuarioDTO?> GetByIdAsync(int id);
        Task<IEnumerable<UsuarioDTO>> GetAllAsync();
        Task<UsuarioDTO> CreateAsync(UsuarioCreateDTO dto);
        Task<bool> UpdateAsync(UsuarioUpdateDTO dto);
        Task<bool> DeleteAsync(UsuarioDeleteDTO dto);
        Task<UsuarioDTO?> GetByCorreoAsync(string correo);
    }
}