using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FacilityOS.API.Common.Exceptions;

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
        catch (ValidationException ex)
        {
            _logger.LogWarning("Validation failed: {Errors}", ex.Errors);
            await WriteValidationProblemDetails(context, ex);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning("Resource not found: {Message}", ex.Message);
            await WriteProblemDetails(context, HttpStatusCode.NotFound, "Resource Not Found", ex.Message);
        }
        catch (ForbiddenException ex)
        {
            _logger.LogWarning("Forbidden access attempt: {Message}", ex.Message);
            await WriteProblemDetails(context, HttpStatusCode.Forbidden, "Forbidden Access", ex.Message);
        }
        catch (ConflictException ex)
        {
            _logger.LogWarning("Conflict occurred: {Message}", ex.Message);
            await WriteProblemDetails(context, HttpStatusCode.Conflict, "Conflict Status", ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Unauthorized access attempt: {Message}", ex.Message);
            await WriteProblemDetails(context, HttpStatusCode.Unauthorized, "Unauthorized", ex.Message);
        }
        catch (BadHttpRequestException ex)
        {
            _logger.LogWarning("Bad request: {Message}", ex.Message);
            await WriteProblemDetails(context, HttpStatusCode.BadRequest, "Bad Request", "Invalid request format.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.");
            await WriteProblemDetails(context, HttpStatusCode.InternalServerError, "Internal Server Error", "An unexpected error occurred.");
        }
    }

    private static async Task WriteValidationProblemDetails(HttpContext context, ValidationException ex)
    {
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = StatusCodes.Status400BadRequest;

        var errorsDictionary = ex.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray()
            );

        var problemDetails = new ValidationProblemDetails(errorsDictionary)
        {
            Type = "https://ietf.org",
            Title = "One or more validation errors occurred.",
            Status = StatusCodes.Status400BadRequest,
            Instance = context.Request.Path
        };

        await SerializeAndWrite(context, problemDetails);
    }

    private static async Task WriteProblemDetails(
        HttpContext context,
        HttpStatusCode statusCode,
        string title,
        string detail)
    {
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;

        var problemDetails = new ProblemDetails
        {
            //Type = $"https://ietf.org.{((int)statusCode == 500 ? "6.1" : "5." + ((int)statusCode % 100))}",
            Type = statusCode switch
            {
                HttpStatusCode.NotFound => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                HttpStatusCode.Forbidden => "https://tools.ietf.org/html/rfc7231#section-6.5.3",
                HttpStatusCode.Conflict => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
                HttpStatusCode.Unauthorized => "https://tools.ietf.org/html/rfc7235#section-3.1",
                HttpStatusCode.BadRequest => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                HttpStatusCode.InternalServerError => "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                _ => "about:blank"
            },
            Title = title,
            Status = (int)statusCode,
            Detail = detail,
            Instance = context.Request.Path
        };

        problemDetails.Extensions.Add("timestamp", DateTime.UtcNow);

        await SerializeAndWrite(context, problemDetails);
    }

    private static async Task SerializeAndWrite(HttpContext context, object problemDetails)
    {
        var json = JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
