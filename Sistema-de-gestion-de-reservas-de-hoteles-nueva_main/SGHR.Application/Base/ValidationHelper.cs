using System.Text.RegularExpressions;

namespace SGHR.Application.Base
{
    public static class ValidationHelper
    {
        public static bool Required(string? value, string fieldName, out string message)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                message = $"El campo '{fieldName}' es obligatorio.";
                return false;
            }

            message = string.Empty;
            return true;
        }

        public static bool IsValidEmail(string? email, out string message)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                message = "El correo electrónico es obligatorio.";
                return false;
            }

            var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            if (!regex.IsMatch(email))
            {
                message = "El formato del correo electrónico no es válido.";
                return false;
            }

            message = string.Empty;
            return true;
        }

        public static bool ValidPhone(string? phone, out string message)
        {
            if (string.IsNullOrWhiteSpace(phone))
            {
                message = "El teléfono es obligatorio.";
                return false;
            }

            var regex = new Regex(@"^\+?[0-9\s-]{7,15}$");
            if (!regex.IsMatch(phone))
            {
                message = "El número de teléfono no es válido.";
                return false;
            }

            message = string.Empty;
            return true;
        }

        public static bool MaxLength(string? value, int maxLength, string fieldName, out string message)
        {
            if (!string.IsNullOrWhiteSpace(value) && value.Length > maxLength)
            {
                message = $"El campo '{fieldName}' no puede tener más de {maxLength} caracteres.";
                return false;
            }

            message = string.Empty;
            return true;
        }

        public static bool NotNull(object? obj, string entityName, out string message)
        {
            if (obj == null)
            {
                message = $"El objeto '{entityName}' no puede ser nulo.";
                return false;
            }

            message = string.Empty;
            return true;
        }

        public static bool IsValidId(int id, string entityName, out string message)
        {
            if (id <= 0)
            {
                message = $"El ID de {entityName} no es válido.";
                return false;
            }

            message = string.Empty;
            return true;
        }

        public static bool IsValidPositiveNumber(decimal value, string fieldName, out string message)
        {
            if (value <= 0)
            {
                message = $"El campo '{fieldName}' debe ser mayor que cero.";
                return false;
            }

            message = string.Empty;
            return true;
        }

        public static bool IsValidPositiveNumber(int value, string fieldName, out string message)
        {
            if (value <= 0)
            {
                message = $"El campo '{fieldName}' debe ser mayor que cero.";
                return false;
            }

            message = string.Empty;
            return true;
        }
    }
}
