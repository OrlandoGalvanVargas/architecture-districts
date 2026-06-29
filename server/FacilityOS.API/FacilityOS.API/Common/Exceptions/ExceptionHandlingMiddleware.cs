using System.Net;
using System.Text.Json;

namespace FacilityOS.API.Common.Exceptions
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (UnauthorizedAccessException ex)
            {
                await WriteError(context, HttpStatusCode.Unauthorized, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                await WriteError(context, HttpStatusCode.Conflict, ex.Message);
            }
            catch (Exception ex)
            {
                await WriteError(context, HttpStatusCode.InternalServerError, "An unexpected error ocurred.");
                Console.WriteLine(ex);
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
