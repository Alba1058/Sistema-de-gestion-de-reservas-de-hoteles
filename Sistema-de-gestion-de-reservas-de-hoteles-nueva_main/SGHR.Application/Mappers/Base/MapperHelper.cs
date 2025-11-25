using SGHR.Domain.Base;

namespace SGHR.Application.Mappers.Base
{
    public static class MapperHelper
    {
        public static string Clean(string? value)
            => value?.Trim() ?? string.Empty;

        public static void SetCreationFields<TEntity>(TEntity entity, string? usuario = null)
            where TEntity : BaseEntity<int>
        {
            entity.FechaCreacion = DateTime.UtcNow;
            entity.UsuarioCreacion = usuario;
            //entity.Estado = true;
            entity.IsDeleted = false;
        }

        public static void SetAuditFields<TEntity>(TEntity entity, string? usuario = null)
            where TEntity : BaseEntity<int>
        {
            entity.FechaModificacion = DateTime.UtcNow;
            entity.UsuarioModificacion = usuario;
        }

        public static void SetDeleteFields<TEntity>(TEntity entity, string? usuario = null)
            where TEntity : BaseEntity<int>
        {
            entity.IsDeleted = true;
            entity.Estado = false;
            entity.FechaEliminacion = DateTime.UtcNow;
            entity.UsuarioEliminacion = usuario;
        }
    }
}
