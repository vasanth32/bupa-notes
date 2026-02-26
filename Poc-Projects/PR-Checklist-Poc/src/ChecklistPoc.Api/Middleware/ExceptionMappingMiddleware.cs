using System.Net.Mime;
using System.Text.Json;
using ChecklistPoc.Api.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace ChecklistPoc.Api.Middleware;

public sealed class ExceptionMappingMiddleware(RequestDelegate next, ILogger<ExceptionMappingMiddleware> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ChecklistPocException ex)
        {
            logger.LogWarning(ex, "Request failed with a known error: {ErrorType}", ex.GetType().Name);
            await WriteProblemDetailsAsync(context, ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Request failed with an unhandled error.");

            var problem = new ProblemDetails
            {
                Title = "Unexpected error",
                Detail = "An unexpected error occurred.",
                Status = StatusCodes.Status500InternalServerError,
                Type = "https://httpstatuses.com/500",
            };

            await WriteProblemDetailsAsync(context, problem);
        }
    }

    private static Task WriteProblemDetailsAsync(HttpContext context, ChecklistPocException exception)
    {
        var (status, title, type) = exception switch
        {
            ValidationException => (StatusCodes.Status400BadRequest, "Validation failed", "https://httpstatuses.com/400"),
            OrderNotFoundException => (StatusCodes.Status404NotFound, "Order not found", "https://httpstatuses.com/404"),
            OrderConflictException => (StatusCodes.Status409Conflict, "Conflict", "https://httpstatuses.com/409"),
            _ => (StatusCodes.Status400BadRequest, "Bad request", "https://httpstatuses.com/400"),
        };

        var problem = new ProblemDetails
        {
            Title = title,
            Detail = exception.Message,
            Status = status,
            Type = type,
        };

        if (exception is ValidationException validationEx && validationEx.Errors.Count > 0)
        {
            problem.Extensions["errors"] = validationEx.Errors;
        }

        return WriteProblemDetailsAsync(context, problem);
    }

    private static Task WriteProblemDetailsAsync(HttpContext context, ProblemDetails problem)
    {
        context.Response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;
        context.Response.ContentType = MediaTypeNames.Application.Json;

        var json = JsonSerializer.Serialize(problem, JsonOptions);
        return context.Response.WriteAsync(json);
    }
}
