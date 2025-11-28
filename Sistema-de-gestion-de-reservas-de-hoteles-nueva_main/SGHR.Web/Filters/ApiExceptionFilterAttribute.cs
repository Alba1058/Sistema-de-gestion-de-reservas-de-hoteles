using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net.Http;

namespace SGHR.Web.Filters
{
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger<ApiExceptionFilterAttribute> _logger;

        public ApiExceptionFilterAttribute(ILogger<ApiExceptionFilterAttribute> logger)
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            var controller = context.RouteData.Values["controller"]?.ToString() ?? "Unknown";
            var action = context.RouteData.Values["action"]?.ToString() ?? "Unknown";

            _logger.LogError(context.Exception,
                "Excepción no controlada en {Controller}.{Action}",
                controller, action);

            string errorMessage;
            string logLevel = "Error";

            switch (context.Exception)
            {
                case HttpRequestException httpEx when httpEx.Message.Contains("404"):
                    errorMessage = "El recurso solicitado no fue encontrado";
                    logLevel = "Warning";
                    break;

                case HttpRequestException httpEx when httpEx.Message.Contains("500"):
                    errorMessage = "Error interno del servidor. Por favor, intente más tarde";
                    break;

                case TaskCanceledException:
                    errorMessage = "La operación tardó demasiado. Por favor, intente nuevamente";
                    logLevel = "Warning";
                    break;

                default:
                    errorMessage = "Ocurrió un error inesperado. Por favor, contacte al administrador";
                    break;
            }

            if (logLevel == "Warning")
                _logger.LogWarning("Error manejado: {Message}", errorMessage);
            else
                _logger.LogError("Error crítico: {Message}", errorMessage);

            if (context.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                context.Result = new JsonResult(new { success = false, message = errorMessage })
                {
                    StatusCode = 500
                };
            }
            else
            {
                context.Result = new RedirectToActionResult("Error", "Home", new { message = errorMessage });
            }

            context.ExceptionHandled = true;
        }
    }
}

