using Microsoft.EntityFrameworkCore;
using SGHR.Domain.Base;
using SGHR.Domain.Repository;
using SGHR.Persistence.Context;
using System.Linq.Expressions;

namespace SGHR.Persistence.Base
{
    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        protected readonly SGHRContext _context;
        protected readonly DbSet<TEntity> _entities;

        protected BaseRepository(SGHRContext context)
        {
            _context = context;
            _entities = _context.Set<TEntity>();
        }

        public virtual async Task<List<TEntity>> GetAllAsync()
        {
            return await _entities.ToListAsync();
        }

        public virtual async Task<TEntity?> GetEntityByIdAsync(int id)
        {
            return await _entities.FindAsync(id);
        }

        public virtual async Task<OperationResult<List<TEntity>>> GetFilteredAsync(Expression<Func<TEntity, bool>> filter)
        {
            var result = new OperationResult<List<TEntity>>();
            try
            {
                result.Data = await _entities.Where(filter).ToListAsync();
                result.Success = true;
                result.Message = "Datos obtenidos correctamente.";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Error al obtener datos filtrados: {ex.Message}";
            }
            return result;
        }

        public virtual async Task<OperationResult<TEntity>> SaveEntityAsync(TEntity entity)
        {
            var result = new OperationResult<TEntity>();
            try
            {
                await _entities.AddAsync(entity);
                await _context.SaveChangesAsync();

                result.Data = entity;
                result.Success = true;
                result.Message = "Entidad guardada correctamente.";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Error al guardar la entidad: {ex.Message}";
            }
            return result;
        }

        public virtual async Task<OperationResult<TEntity>> UpdateEntityAsync(TEntity entity)
        {
            var result = new OperationResult<TEntity>();
            try
            {
                _entities.Update(entity);
                await _context.SaveChangesAsync();

                result.Data = entity;
                result.Success = true;
                result.Message = "Entidad actualizada correctamente.";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Error al actualizar la entidad: {ex.Message}";
            }
            return result;
        }

        public virtual async Task<OperationResult<bool>> DeleteEntityAsync(TEntity entity)
        {
            var result = new OperationResult<bool>();
            try
            {
                _context.Entry(entity).Property("IsDeleted").CurrentValue = true;
                _context.Entry(entity).Property("Estado").CurrentValue = false;
                _context.Entry(entity).Property("FechaEliminacion").CurrentValue = DateTime.UtcNow;
                _context.Entry(entity).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                result.Data = true;
                result.Success = true;
                result.Message = "Entidad eliminada correctamente.";
            }
            catch (Exception ex)
            {
                result.Data = false;
                result.Success = false;
                result.Message = $"Error al eliminar la entidad: {ex.Message}";
            }
            return result;
        }

        public virtual async Task<OperationResult<bool>> RestoreEntityAsync(TEntity entity)
        {
            var result = new OperationResult<bool>();
            try
            {
                _context.Entry(entity).Property("IsDeleted").CurrentValue = false;
                _context.Entry(entity).Property("Estado").CurrentValue = true;
                _context.Entry(entity).Property("FechaEliminacion").CurrentValue = null;
                _context.Entry(entity).Property("FechaModificacion").CurrentValue = DateTime.UtcNow;
                _context.Entry(entity).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                result.Data = true;
                result.Success = true;
                result.Message = "Entidad restaurada correctamente.";
            }
            catch (Exception ex)
            {
                result.Data = false;
                result.Success = false;
                result.Message = $"Error al restaurar la entidad: {ex.Message}";
            }
            return result;
        }

        public virtual async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> filter)
        {
            return await _entities.AnyAsync(filter);
        }
    }
}
