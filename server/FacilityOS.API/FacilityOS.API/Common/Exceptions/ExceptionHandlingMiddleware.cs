using System.Net;
using System.Text.Json;

namespace FacilityOS.API.Common.Exceptions
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
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning("Resource not found: {Message}", ex.Message);
                await WriteError(context, HttpStatusCode.NotFound, ex.Message);
            }
            catch (ForbiddenException ex)
            {
                _logger.LogWarning("Forbidden access attempt: {Message}", ex.Message);
                await WriteError(context, HttpStatusCode.Forbidden, ex.Message);
            }
            catch (ConflictException ex)
            {
                _logger.LogWarning("Conflict occurred: {Message}", ex.Message);
                await WriteError(context, HttpStatusCode.Conflict, ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Unauthorized access attempt: {Message}", ex.Message);
                await WriteError(context, HttpStatusCode.Unauthorized, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Business rule conflict: {Message}", ex.Message);
                await WriteError(context, HttpStatusCode.Conflict, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred.");
                await WriteError(context, HttpStatusCode.InternalServerError, "An unexpected error occurred.");
            }
        }

        private static async Task WriteError(HttpContext context, HttpStatusCode statusCode, string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = JsonSerializer.Serialize(new { error = message });
            await context.Response.WriteAsync(response);
        }
    }
}