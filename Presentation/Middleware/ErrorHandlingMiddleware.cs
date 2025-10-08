using HotelManagement.Aplicacion.Exceptions;
using System.Net;
using System.Text.Json;

namespace HotelManagement.Presentacion.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;
            var result = string.Empty;

            switch (exception)
            {
                case NotFoundException notFoundException:
                    code = HttpStatusCode.NotFound;
                    result = JsonSerializer.Serialize(new
                    {
                        error = "Recurso no encontrado",
                        message = notFoundException.Message
                    });
                    _logger.LogWarning(notFoundException, "Recurso no encontrado");
                    break;

                case BadRequestException badRequestException:
                    code = HttpStatusCode.BadRequest;
                    result = JsonSerializer.Serialize(new
                    {
                        error = "Solicitud incorrecta",
                        message = badRequestException.Message
                    });
                    _logger.LogWarning(badRequestException, "Solicitud incorrecta");
                    break;

                case ValidationException validationException:
                    code = HttpStatusCode.BadRequest;
                    result = JsonSerializer.Serialize(new
                    {
                        error = "Errores de validación",
                        message = validationException.Message,
                        errors = validationException.Errors
                    });
                    _logger.LogWarning(validationException, "Errores de validación");
                    break;

                case ConflictException conflictException:
                    code = HttpStatusCode.Conflict;
                    result = JsonSerializer.Serialize(new
                    {
                        error = "Conflicto",
                        message = conflictException.Message
                    });
                    _logger.LogWarning(conflictException, "Conflicto");
                    break;

                default:
                    code = HttpStatusCode.InternalServerError;
                    result = JsonSerializer.Serialize(new
                    {
                        error = "Error interno del servidor",
                        message = "Ocurrió un error inesperado. Por favor, contacte al administrador."
                    });
                    _logger.LogError(exception, "Error interno del servidor");
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}
