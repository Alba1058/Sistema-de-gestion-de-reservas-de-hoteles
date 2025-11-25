using System.Reflection;

namespace SGHR.Application.Base
{
    public static class EntityValidationHelper
    {
        public static bool ValidateEntityExists<TEntity>(
            TEntity? entity, 
            string entityName,
            out string message) where TEntity : class
        {
            if (entity == null)
            {
                message = $"{entityName} no encontrado.";
                return false;
            }

            var isDeletedProperty = entity.GetType().GetProperty("IsDeleted");
            if (isDeletedProperty != null && isDeletedProperty.GetValue(entity) is bool isDeleted && isDeleted)
            {
                message = $"{entityName} no encontrado.";
                return false;
            }

            message = string.Empty;
            return true;
        }

        public static bool ValidateEntityNotDeleted<TEntity>(
            TEntity? entity, 
            string entityName,
            out string message) where TEntity : class
        {
            if (entity == null)
            {
                message = $"{entityName} no encontrado.";
                return false;
            }

            var isDeletedProperty = entity.GetType().GetProperty("IsDeleted");
            if (isDeletedProperty != null && isDeletedProperty.GetValue(entity) is bool isDeleted && isDeleted)
            {
                message = $"{entityName} ya está eliminado.";
                return false;
            }

            message = string.Empty;
            return true;
        }

        public static bool ValidateRelatedEntity<TEntity>(
            TEntity? entity,
            string entityName,
            out string message) where TEntity : class
        {
            if (entity == null)
            {
                message = $"La {entityName} especificada no existe.";
                return false;
            }

            var isDeletedProperty = entity.GetType().GetProperty("IsDeleted");
            if (isDeletedProperty != null && isDeletedProperty.GetValue(entity) is bool isDeleted && isDeleted)
            {
                message = $"La {entityName} especificada no existe.";
                return false;
            }

            message = string.Empty;
            return true;
        }
    }
}

