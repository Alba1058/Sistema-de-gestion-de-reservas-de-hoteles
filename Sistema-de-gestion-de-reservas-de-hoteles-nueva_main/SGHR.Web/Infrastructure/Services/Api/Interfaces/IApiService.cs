using SGHR.Domain.Base;

namespace SGHR.Web.Infrastructure.Services.Api.Interfaces
{
    public interface IApiService<TDto, TCreateDto, TUpdateDto, TDeleteDto>
    {
        Task<OperationResult<List<TDto>>> GetAllAsync();
        Task<OperationResult<TDto>> GetByIdAsync(int id);
        Task<OperationResult<TDto>> CreateAsync(TCreateDto dto);
        Task<OperationResult<TDto>> UpdateAsync(TUpdateDto dto);
        Task<OperationResult<bool>> DeleteAsync(int id);
    }
}

