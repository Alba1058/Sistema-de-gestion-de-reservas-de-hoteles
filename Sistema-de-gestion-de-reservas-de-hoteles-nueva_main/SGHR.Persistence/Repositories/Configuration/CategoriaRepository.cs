using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGHR.Domain.Base;
using SGHR.Domain.Entities.Configuration; 
using SGHR.Persistence.Base;
using SGHR.Persistence.Context;
using SGHR.Persistence.Interfaces.Configuration;

namespace SGHR.Persistence.Repositories.Configuration
{
    public sealed class CategoriaRepository : BaseRepository<Categoria>, ICategoriaRepository
    {
        private readonly ILogger<CategoriaRepository> _logger;

        public CategoriaRepository(SGHRContext context, ILogger<CategoriaRepository> logger)
            : base(context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task<List<Categoria>> GetCategoriasActivas()
        {
            try
            {
                var categorias = await _context.Categorias
                    .Where(c => c.IsDeleted == false)
                    .ToListAsync();

                _logger.LogInformation("Se obtuvieron {Count} categorías activas", categorias.Count);
                return categorias;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener categorías activas.");
                return new List<Categoria>();
            }
        }

        public override async Task<OperationResult<Categoria>> SaveEntityAsync(Categoria entity)
        {
            try
            {
                var result = await base.SaveEntityAsync(entity);

                if (result.Success)
                    _logger.LogInformation("Categoría guardada correctamente: {Nombre} (Id: {Id})", entity.Nombre, entity.Id);
                else
                    _logger.LogWarning("Error al guardar categoría: {Mensaje}", result.Message);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno al guardar categoría.");
                return OperationResult<Categoria>.Fail("Error interno al guardar la categoría.");
            }
        }

        public override async Task<OperationResult<Categoria>> UpdateEntityAsync(Categoria entity)
        {
            try
            {
                var result = await base.UpdateEntityAsync(entity);

                if (result.Success)
                    _logger.LogInformation("Categoría actualizada correctamente: {Nombre} (Id: {Id})", entity.Nombre, entity.Id);
                else
                    _logger.LogWarning("Error al actualizar categoría: {Mensaje}", result.Message);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno al actualizar categoría.");
                return OperationResult<Categoria>.Fail("Error interno al actualizar la categoría.");
            }
        }

        public override async Task<OperationResult<bool>> DeleteEntityAsync(Categoria entity)
        {
            try
            {
                var result = await base.DeleteEntityAsync(entity);

                if (result.Success)
                    _logger.LogInformation("Categoría eliminada correctamente: {Nombre} (Id: {Id})", entity.Nombre, entity.Id);
                else
                    _logger.LogWarning("Error al eliminar categoría: {Mensaje}", result.Message);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno al eliminar categoría.");
                return OperationResult<bool>.Fail("Error interno al eliminar la categoría.");
            }
        }
    }
}