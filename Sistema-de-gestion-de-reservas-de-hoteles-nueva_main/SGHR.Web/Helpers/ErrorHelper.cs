using SGHR.Domain.Base;

namespace SGHR.Web.Helpers
{
    public static class ErrorHelper
    {
        public static string? SanitizeErrorMessage(string? message)
        {
            if (string.IsNullOrEmpty(message))
                return null;

            if (message.Contains("Exception") || message.Contains("Stack") || 
                message.Contains("at ") || message.Contains("System."))
                return "Ocurrió un error interno. Por favor, contacte al administrador";

            return message;
        }

        public static bool IsSuccess<T>(OperationResult<T>? result, out string? errorMessage)
        {
            errorMessage = null;
            
            if (result == null)
            {
                errorMessage = "No se recibió respuesta del servidor";
                return false;
            }
            
            if (!result.Success)
            {
                errorMessage = SanitizeErrorMessage(result.Message) ?? "Operación fallida";
                return false;
            }
            
            return true;
        }

        public static bool IsValidId(int id, out string? errorMessage)
        {
            errorMessage = null;
            
            if (id <= 0)
            {
                errorMessage = "El ID proporcionado no es válido";
                return false;
            }
            
            return true;
        }
    }

    public static class SuccessMessages
    {
        public const string Created = "Registro creado exitosamente";
        public const string Updated = "Registro actualizado exitosamente";
        public const string Deleted = "Registro eliminado exitosamente";
        public const string Loaded = "Datos cargados correctamente";
        
        public static string GetCreatedMessage(string entityName) => 
            $"{entityName} creado(a) exitosamente";
        
        public static string GetUpdatedMessage(string entityName) => 
            $"{entityName} actualizado(a) exitosamente";
        
        public static string GetDeletedMessage(string entityName) => 
            $"{entityName} eliminado(a) exitosamente";
    }
}

