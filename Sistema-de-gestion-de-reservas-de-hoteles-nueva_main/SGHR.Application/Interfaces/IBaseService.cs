using SGHR.Domain.Base;

namespace SGHR.Application.Interfaces
{
    public interface IBaseService<TCreate, TUpdate, TDelete, TResultDTO>
    {
        Task<OperationResult<List<TResultDTO>>> GetAllAsync();
        Task<OperationResult<TResultDTO>> GetByIdAsync(int id);

        Task<OperationResult<TResultDTO>> CreateAsync(TCreate dto);
        Task<OperationResult<TResultDTO>> UpdateAsync(TUpdate dto);

        Task<OperationResult<bool>> RemoveAsync(TDelete dto);
    }
}