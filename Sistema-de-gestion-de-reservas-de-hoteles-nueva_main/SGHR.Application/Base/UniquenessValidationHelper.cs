using System.Linq.Expressions;

namespace SGHR.Application.Base
{
    public static class UniquenessValidationHelper
    {
        public static async Task<(bool IsValid, string Message)> ValidateUniquenessAsync<TEntity>(
            Func<Expression<Func<TEntity, bool>>, Task<bool>> existsAsync,
            Expression<Func<TEntity, bool>> predicate,
            string fieldName,
            string entityName) where TEntity : class
        {
            var exists = await existsAsync(predicate);
            if (exists)
                return (false, $"Ya existe un {entityName} con este {fieldName}.");

            return (true, string.Empty);
        }

        public static async Task<(bool IsValid, string Message)> ValidateUniquenessForUpdateAsync<TEntity>(
            Func<Expression<Func<TEntity, bool>>, Task<bool>> existsAsync,
            Expression<Func<TEntity, bool>> predicate,
            string fieldName,
            string entityName) where TEntity : class
        {
            var exists = await existsAsync(predicate);
            if (exists)
                return (false, $"Otro {entityName} ya usa este {fieldName}.");

            return (true, string.Empty);
        }
    }
}

