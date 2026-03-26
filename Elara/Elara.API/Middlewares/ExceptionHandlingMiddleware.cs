using Elara.Application.Exceptions;
using Elara.Application.Models;
using System.Text.Json;

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
                Message = message
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
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
                        Message = "Validation failed."
                    }),
                KeyNotFoundException => (
                    StatusCodes.Status404NotFound,
                    new ErrorResponse { Message = exception.Message }),
                UnauthorizedAccessException => (
                    StatusCodes.Status401Unauthorized,
                    new ErrorResponse { Message = exception.Message }),
                InvalidOperationException => (
                    StatusCodes.Status400BadRequest,
                    new ErrorResponse { Message = exception.Message }),
                _ => (
                    StatusCodes.Status500InternalServerError,
                    new ErrorResponse
                    {
                        Message = "An unexpected error occurred."
                    })
            };

            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
