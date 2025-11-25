namespace SGHR.Application.Base
{
    public static class EnumValidationHelper
    {
        public static bool ValidateEnum<TEnum>(TEnum value, string fieldName, out string message) where TEnum : struct, Enum
        {
            if (!Enum.IsDefined(typeof(TEnum), value))
            {
                message = $"El {fieldName} no es válido.";
                return false;
            }

            message = string.Empty;
            return true;
        }
    }
}

