using System.Text.Json;

namespace Demo.OrderApi.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var (status, code, message, logLevel) = MapException(ex);

            _logger.Log(logLevel, ex, "Unhandled exception mapped. Status={Status} Code={Code} TraceId={TraceId}", status, code, context.TraceIdentifier);

            context.Response.Clear();
            context.Response.StatusCode = status;
            context.Response.ContentType = "application/json";

            var payload = new
            {
                traceId = context.TraceIdentifier,
                status,
                error = new
                {
                    code,
                    message
                }
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(payload, JsonOptions));
        }
    }

    private static (int Status, string Code, string Message, LogLevel LogLevel) MapException(Exception ex) =>
        ex switch
        {
            KeyNotFoundException => (StatusCodes.Status404NotFound, "not_found", ex.Message, LogLevel.Information),
            ArgumentOutOfRangeException or ArgumentNullException or ArgumentException =>
                (StatusCodes.Status400BadRequest, "bad_request", ex.Message, LogLevel.Information),
            InvalidOperationException =>
                (StatusCodes.Status500InternalServerError, "internal_error", ex.Message, LogLevel.Error),
            _ => (StatusCodes.Status500InternalServerError, "internal_error", "Unexpected error.", LogLevel.Error)
        };
}

