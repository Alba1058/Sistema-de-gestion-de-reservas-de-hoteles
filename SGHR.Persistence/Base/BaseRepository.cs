using SGHR.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SGHR.Domain.Base;
using SGHR.Domain.Repository;

namespace SGHR.Persistence.Base
{
    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity>
     where TEntity : BaseEntity<int>

    {
        private readonly SGHRContext _context;
        private DbSet<TEntity> Entity { get; set; }
        public BaseRepository(SGHRContext context)
        {
            _context = context;
            Entity = _context.Set<TEntity>();
        }
        public virtual async Task<OperationResult> SaveEntityAsync(TEntity entity)
        {
            OperationResult result = new OperationResult();

            try
            {
                Entity.Add(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Ocurrio un error guardando los datos: {ex.Message}";
            }
            return result;
        }

        public virtual async Task<OperationResult> UpdateEntityAsync(TEntity entity)
        {
            OperationResult result = new OperationResult();
            try
            {
                Entity.Update(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Ocurrio un error guardando los datos: {ex.Message}";
            }
            return result;
        }

        public virtual async Task<OperationResult> DeleteEntityAsync(TEntity entity)
        {
            var result = new OperationResult();
            try
            {
                entity.IsDeleted = true;
                entity.FechaModificacion = DateTime.Now;
                Entity.Update(entity);
                await _context.SaveChangesAsync();
                result.Data = entity;
            }

            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Ocurrio un error eliminando los datos: {ex.Message}";
            }

            return result;
        }

        public virtual async Task<OperationResult> RestoreEntityAsync(TEntity entity)
        {
            var result = new OperationResult();
            try
            {
                entity.IsDeleted = false; 
                entity.FechaModificacion = DateTime.Now;
                Entity.Update(entity);
                await _context.SaveChangesAsync();
                result.Data = entity;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Error restaurando: {ex.Message}";
            }
            return result;
        }

        public virtual async Task<OperationResult> GetFilteredAsync(Expression<Func<TEntity, bool>> filter)
        {
            OperationResult result = new OperationResult();

            try
            {
                var datos = await Entity.Where(filter).ToListAsync();
                result.Data = datos;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Ocurrio un error obteniendo : {ex.Message}";
            }

            return result;
        }

        public virtual async Task<TEntity?> GetEntityByIdAsync(int id)
        {
            return await Entity.FindAsync(id);
        }

        public virtual async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> filter)
        {

            return await Entity.AnyAsync(filter);
        }
        public virtual async Task<List<TEntity>> GetAllAsync()
        {
            return await Entity.ToListAsync();
        }

    }
}
