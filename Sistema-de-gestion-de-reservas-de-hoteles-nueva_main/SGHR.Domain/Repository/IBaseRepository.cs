using SGHR.Domain.Base;
using System.Linq.Expressions;

namespace SGHR.Domain.Repository
{

    public interface IBaseRepository<TEntity> where TEntity : class
    {
        Task<List<TEntity>> GetAllAsync();
        Task<TEntity?> GetEntityByIdAsync(int id);
        Task<OperationResult<List<TEntity>>> GetFilteredAsync(Expression<Func<TEntity, bool>> filter);

        Task<OperationResult<TEntity>> SaveEntityAsync(TEntity entity);
        Task<OperationResult<TEntity>> UpdateEntityAsync(TEntity entity);
        Task<OperationResult<bool>> DeleteEntityAsync(TEntity entity);
        Task<OperationResult<bool>> RestoreEntityAsync(TEntity entity);

        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> filter);
    }
}