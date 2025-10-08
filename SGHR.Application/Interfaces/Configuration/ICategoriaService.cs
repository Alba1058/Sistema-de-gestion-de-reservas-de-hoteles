using SGHR.Application.DTOs.Configuration.Categoria;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGHR.Application.Interfaces.Configuration
{
    public interface ICategoriaService
    {
        Task<CategoriaDTO?> GetByIdAsync(int id);
        Task<IEnumerable<CategoriaDTO>> GetAllAsync();
        Task<CategoriaDTO> CreateAsync(CreateCategoriaDTO dto);
        Task<bool> UpdateAsync(UpdateCategoriaDTO dto);
        Task<bool> DeleteAsync(DeleteCategoriaDTO dto);
        Task<CategoriaDTO?> RestoreAsync(int id);
    }
}