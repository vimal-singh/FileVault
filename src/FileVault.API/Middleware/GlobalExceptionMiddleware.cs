using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;

namespace FileVault.API.Middleware;

/// <summary>
/// Catches all unhandled exceptions and maps them to RFC 7807 ProblemDetails responses.
/// This is the single place in the entire application where exception → HTTP mapping lives.
/// No try/catch blocks in controllers or handlers.
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger)
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
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // TODO: Map domain exceptions to HTTP status codes here.
        // Example: FileNotFoundException → 404, UnauthorizedFileAccessException → 403
        // For now, all unhandled exceptions return 500.

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var problem = new
        {
            type    = "https://filevault.io/errors/internal-server-error",
            title   = "An unexpected error occurred.",
            status  = 500,
            detail  = exception.Message,
            traceId = context.TraceIdentifier
        };

        return context.Response.WriteAsync(
            JsonSerializer.Serialize(problem));
    }
}
