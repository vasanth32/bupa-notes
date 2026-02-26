using System.Text;
using System.Text.Json;
using ChecklistPoc.Api.Domain.Orders;
using ChecklistPoc.Api.Exceptions;
using ChecklistPoc.Api.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ChecklistPoc.UnitTests.Middleware;

public sealed class ExceptionMappingMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_WhenValidationException_Returns400WithErrorsExtension()
    {
        var middleware = CreateMiddlewareThatThrows(new ValidationException(
            "bad input",
            new Dictionary<string, string[]>
            {
                ["CustomerId"] = ["required"],
            }));

        var context = new DefaultHttpContext();
        await using var body = new MemoryStream();
        context.Response.Body = body;

        await middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);

        var json = await ReadBodyAsync(body);
        using var doc = JsonDocument.Parse(json);
        Assert.Equal(400, doc.RootElement.GetProperty("status").GetInt32());
        Assert.Equal("Validation failed", doc.RootElement.GetProperty("title").GetString());
        Assert.True(doc.RootElement.TryGetProperty("errors", out var errors));
        Assert.True(errors.TryGetProperty("CustomerId", out _));
    }

    [Fact]
    public async Task InvokeAsync_WhenOrderNotFound_Returns404()
    {
        var middleware = CreateMiddlewareThatThrows(new OrderNotFoundException(new OrderId(Guid.NewGuid())));

        var context = new DefaultHttpContext();
        await using var body = new MemoryStream();
        context.Response.Body = body;

        await middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status404NotFound, context.Response.StatusCode);

        var json = await ReadBodyAsync(body);
        using var doc = JsonDocument.Parse(json);
        Assert.Equal(404, doc.RootElement.GetProperty("status").GetInt32());
        Assert.Equal("Order not found", doc.RootElement.GetProperty("title").GetString());
    }

    [Fact]
    public async Task InvokeAsync_WhenUnhandledException_Returns500()
    {
        var middleware = CreateMiddlewareThatThrows(new InvalidOperationException("boom"));

        var context = new DefaultHttpContext();
        await using var body = new MemoryStream();
        context.Response.Body = body;

        await middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);

        var json = await ReadBodyAsync(body);
        using var doc = JsonDocument.Parse(json);
        Assert.Equal(500, doc.RootElement.GetProperty("status").GetInt32());
        Assert.Equal("Unexpected error", doc.RootElement.GetProperty("title").GetString());
    }

    private static ExceptionMappingMiddleware CreateMiddlewareThatThrows(Exception ex)
    {
        RequestDelegate next = _ => Task.FromException(ex);
        var loggerFactory = LoggerFactory.Create(_ => { });
        var logger = loggerFactory.CreateLogger<ExceptionMappingMiddleware>();
        return new ExceptionMappingMiddleware(next, logger);
    }

    private static async Task<string> ReadBodyAsync(MemoryStream body)
    {
        body.Position = 0;
        using var reader = new StreamReader(body, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, leaveOpen: true);
        return await reader.ReadToEndAsync();
    }
}

