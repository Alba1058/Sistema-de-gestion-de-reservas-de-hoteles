using SGHR.Application.DTOs.Configuration.Categoria;
using SGHR.Domain.Base;

namespace SGHR.Application.Interfaces.Configuration
{
    public interface ICategoriaService : IBaseService<CreateCategoriaDTO, UpdateCategoriaDTO, DeleteCategoriaDTO, CategoriaDTO>
    {
        Task<OperationResult<List<CategoriaDTO>>> GetCategoriasActivasAsync();
    }
}
