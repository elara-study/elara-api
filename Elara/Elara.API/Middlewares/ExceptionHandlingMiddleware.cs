using Elara.Application.Exceptions;
using Elara.Application.Models;
using System.Text.Json;
using Elara.Domain.Constants;

namespace Elara.API.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);

                if (!context.Response.HasStarted
                    && context.Response.ContentLength is null
                    && (context.Response.StatusCode == StatusCodes.Status401Unauthorized
                        || context.Response.StatusCode == StatusCodes.Status403Forbidden))
                {
                    await HandleAuthResponsesAsync(context);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Unhandled exception occurred while processing request.");
                await HandleExceptionAsync(context, exception);
            }
        }

        private static async Task HandleAuthResponsesAsync(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            var message = context.Response.StatusCode switch
            {
                StatusCodes.Status401Unauthorized => context.Response.Headers.ContainsKey("Token-Expired")
                    ? "Token has expired."
                    : "Authentication is required.",
                StatusCodes.Status403Forbidden => "You don't have permission to access this resource.",
                _ => "Unauthorized access."
            };

            var response = new ErrorResponse
            {
                Status = ResponseStatus.Error,
                Message = message
            };

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var (statusCode, response) = exception switch
            {
                ValidationException validationException => (
                    StatusCodes.Status400BadRequest,
                    (object)new ErrorResponse
                    {
                        Status = ResponseStatus.ValidationError,
                        Message = "Validation failed.",
                        Data = validationException.Errors
                    }),
                KeyNotFoundException => (
                    StatusCodes.Status404NotFound,
                    ErrorResponse.Create(exception.Message)),
                UnauthorizedAccessException => (
                    StatusCodes.Status401Unauthorized,
                    ErrorResponse.Create(exception.Message)),
                InvalidOperationException => (
                    StatusCodes.Status400BadRequest,
                    ErrorResponse.Create(exception.Message)),
                _ => (
                    StatusCodes.Status500InternalServerError,
                    new ErrorResponse
                    {
                        Status = ResponseStatus.Error,
                        Message = "An unexpected error occurred."
                    })
            };

            context.Response.StatusCode = statusCode;
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }
    }
}
