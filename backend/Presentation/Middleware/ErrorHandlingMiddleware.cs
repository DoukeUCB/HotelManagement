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
                _logger.LogError(ex, "Error no controlado: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;
            var result = string.Empty;

            switch (exception)
            {
                case ValidationException validationException:
                    code = HttpStatusCode.BadRequest;
                    result = JsonSerializer.Serialize(new
                    {
                        message = "Errores de validación",
                        errors = validationException.Errors
                    });
                    break;

                case NotFoundException _:
                    code = HttpStatusCode.NotFound;
                    result = JsonSerializer.Serialize(new { message = exception.Message });
                    break;

                case BadRequestException _:
                    code = HttpStatusCode.BadRequest;
                    result = JsonSerializer.Serialize(new { message = exception.Message });
                    break;

                case ConflictException _:
                    code = HttpStatusCode.Conflict;
                    result = JsonSerializer.Serialize(new { message = exception.Message });
                    break;

                default:
                    result = JsonSerializer.Serialize(new
                    {
                        message = "Error interno del servidor",
                        details = exception.Message
                    });
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            return context.Response.WriteAsync(result);
        }
    }
}
