using SGHR.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SGHR.Domain.Repositoy
{

    public interface IBaseRepository<TEntity> where TEntity : class
    {
        Task<List<TEntity>> GetAllAsync();
        Task<TEntity?> GetEntityByIdAsync(int id);
        Task<OperationResult> GetFilteredAsync(Expression<Func<TEntity, bool>> filter);

        Task<OperationResult> SaveEntityAsync(TEntity entity);
        Task<OperationResult> UpdateEntityAsync(TEntity entity);
        Task<OperationResult> DeleteEntityAsync(TEntity entity);
        Task<OperationResult> RestoreEntityAsync(TEntity entity);

        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> filter);
    }
}