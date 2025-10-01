using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGHR.Application.Validators
{
    public static class ValidationHelper
    {
        public static bool NotNull<T>(T entity, string entityName, out string errorMessage)
        {
            if (entity == null)
            {
                errorMessage = $"{entityName} no puede ser nulo.";
                return false;
            }
            errorMessage = string.Empty;
            return true;
        }

        public static bool Required(string? value, string fieldName, out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                errorMessage = $"{fieldName} es obligatorio.";
                return false;
            }
            errorMessage = string.Empty;
            return true;
        }

        public static bool GreaterThanZero(int value, string fieldName, out string errorMessage)
        {
            if (value <= 0)
            {
                errorMessage = $"{fieldName} debe ser mayor que cero.";
                return false;
            }
            errorMessage = string.Empty;
            return true;
        }

        public static bool GreaterThanZero(decimal value, string fieldName, out string errorMessage)
        {
            if (value <= 0)
            {
                errorMessage = $"{fieldName} debe ser mayor que cero.";
                return false;
            }
            errorMessage = string.Empty;
            return true;
        }

        public static bool MaxLength(string? value, int maxLength, string fieldName, out string errorMessage)
        {
            if (!string.IsNullOrEmpty(value) && value.Length > maxLength)
            {
                errorMessage = $"{fieldName} no puede superar los {maxLength} caracteres.";
                return false;
            }
            errorMessage = string.Empty;
            return true;
        }
    }
}