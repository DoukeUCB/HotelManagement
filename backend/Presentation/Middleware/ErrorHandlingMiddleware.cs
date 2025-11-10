using HotelManagement.Aplicacion.Exceptions;
using HotelManagement.DTOs;
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
                _logger.LogError(ex, "Error capturado: {Type} - {Message}", ex.GetType().Name, ex.Message);
                await HandleExceptionAsync(context, ex, _logger);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception, ILogger logger)
        {
            var statusCode = HttpStatusCode.InternalServerError;
            ApiResponse response;

            switch (exception)
            {
                case ValidationException validationException:
                    statusCode = HttpStatusCode.UnprocessableEntity; // 422
                    logger.LogWarning("ValidationException capturada con {Count} errores", validationException.Errors.Count);
                    foreach (var error in validationException.Errors)
                    {
                        logger.LogWarning("  - Campo '{Field}': {Messages}", error.Key, string.Join(", ", error.Value));
                    }
                    response = ApiResponse.ErrorResponse(
                        "Errores de validación en los campos proporcionados",
                        (int)statusCode,
                        validationException.Errors
                    );
                    break;

                case NotFoundException notFoundException:
                    statusCode = HttpStatusCode.NotFound; // 404
                    response = ApiResponse.ErrorResponse(
                        notFoundException.Message,
                        (int)statusCode,
                        string.IsNullOrEmpty(notFoundException.Field) 
                            ? null 
                            : new Dictionary<string, List<string>> 
                            { 
                                { notFoundException.Field, new List<string> { notFoundException.Message } } 
                            }
                    );
                    break;

                case BadRequestException badRequestException:
                    statusCode = HttpStatusCode.BadRequest; // 400
                    response = ApiResponse.ErrorResponse(
                        badRequestException.Message,
                        (int)statusCode,
                        string.IsNullOrEmpty(badRequestException.Field) 
                            ? null 
                            : new Dictionary<string, List<string>> 
                            { 
                                { badRequestException.Field, new List<string> { badRequestException.Message } } 
                            }
                    );
                    break;

                case ConflictException conflictException:
                    statusCode = HttpStatusCode.Conflict; // 409
                    response = ApiResponse.ErrorResponse(
                        conflictException.Message,
                        (int)statusCode,
                        string.IsNullOrEmpty(conflictException.Field) 
                            ? null 
                            : new Dictionary<string, List<string>> 
                            { 
                                { conflictException.Field, new List<string> { conflictException.Message } } 
                            }
                    );
                    break;

                case UnauthorizedException unauthorizedException:
                    statusCode = HttpStatusCode.Unauthorized; // 401
                    response = ApiResponse.ErrorResponse(
                        unauthorizedException.Message,
                        (int)statusCode
                    );
                    break;

                case ForbiddenException forbiddenException:
                    statusCode = HttpStatusCode.Forbidden; // 403
                    response = ApiResponse.ErrorResponse(
                        forbiddenException.Message,
                        (int)statusCode
                    );
                    break;

                default:
                    statusCode = HttpStatusCode.InternalServerError; // 500
                    response = ApiResponse.ErrorResponse(
                        "Error interno del servidor. Por favor, contacte al administrador.",
                        (int)statusCode,
                        new Dictionary<string, List<string>> 
                        { 
                            { "details", new List<string> { exception.Message } } 
                        }
                    );
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var result = JsonSerializer.Serialize(response, options);
            return context.Response.WriteAsync(result);
        }
    }
}
