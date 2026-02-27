using Apollo.Customer.Mock.Application.Exceptions;
using Apollo.Customer.Mock.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Apollo.Customer.Mock.WebApi.Middleware;

/// <summary>
/// Global exception handling middleware.
/// </summary>
public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionHandlingMiddleware"/> class.
    /// </summary>
    /// <param name="next">Next request delegate.</param>
    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Invokes the middleware.
    /// </summary>
    /// <param name="context">HTTP context.</param>
    /// <returns>Task.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (DomainValidationException ex)
        {
            await WriteProblemDetailsAsync(
                context,
                StatusCodes.Status400BadRequest,
                "Domain validation failed.",
                ex.Message);
        }
        catch (CustomerNotFoundException ex)
        {
            await WriteProblemDetailsAsync(
                context,
                StatusCodes.Status404NotFound,
                "Customer not found.",
                ex.Message);
        }
        catch (CustomerPersistenceException ex)
        {
            await WriteProblemDetailsAsync(
                context,
                StatusCodes.Status500InternalServerError,
                "Persistence error.",
                ex.Message);
        }
    }

    private static async Task WriteProblemDetailsAsync(
        HttpContext context,
        int statusCode,
        string title,
        string detail)
    {
        context.Response.StatusCode = statusCode;
        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
        };

        await context.Response.WriteAsJsonAsync(
            problem,
            options: null,
            contentType: "application/problem+json",
            cancellationToken: context.RequestAborted);
    }
}

