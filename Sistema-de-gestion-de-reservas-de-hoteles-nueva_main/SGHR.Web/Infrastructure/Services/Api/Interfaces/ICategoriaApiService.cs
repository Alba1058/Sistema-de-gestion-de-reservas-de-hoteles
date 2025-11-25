using SGHR.Application.DTOs.Configuration.Categoria;
using SGHR.Domain.Base;

namespace SGHR.Web.Infrastructure.Services.Api.Interfaces
{
    public interface ICategoriaApiService : IApiService<CategoriaDTO, CreateCategoriaDTO, UpdateCategoriaDTO, DeleteCategoriaDTO>
    {
    }
}

