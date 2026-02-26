using System.Net;
using System.Net.Http.Json;
using Demo.OrderApi.Models;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Demo.OrderApi.IntegrationTests;

public sealed class OrderApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public OrderApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        // Real ASP.NET Core pipeline in-memory (routing, model binding, DI, filters, etc.)
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_orders_id_returns_200()
    {
        var response = await _client.GetAsync("/orders/1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var order = await response.Content.ReadFromJsonAsync<Order>();
        Assert.NotNull(order);
        Assert.Equal(1, order!.Id);
        Assert.False(string.IsNullOrWhiteSpace(order.ProductCode));
    }

    [Fact]
    public async Task Get_invalid_id_returns_404()
    {
        // "Invalid" here means "not found" in our fake repository.
        var response = await _client.GetAsync("/orders/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var raw = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"code\":\"not_found\"", raw);
        Assert.Contains("\"status\":404", raw);
        Assert.Contains("\"traceId\"", raw);
    }

    [Fact]
    public async Task Get_bad_id_returns_400_with_error_json()
    {
        var response = await _client.GetAsync("/orders/0");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var raw = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"code\":\"bad_request\"", raw);
        Assert.Contains("\"status\":400", raw);
        Assert.Contains("\"traceId\"", raw);
    }

    [Fact]
    public async Task Post_orders_returns_201()
    {
        var request = new CreateOrderRequest(ProductCode: "SKU-POST", Quantity: 2);

        var response = await _client.PostAsJsonAsync("/orders", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var created = await response.Content.ReadFromJsonAsync<Order>();
        Assert.NotNull(created);
        Assert.True(created!.Id > 0);
        Assert.Equal("SKU-POST", created.ProductCode);
        Assert.Equal(2, created.Quantity);
    }

    [Fact]
    public async Task Json_response_format_is_as_expected()
    {
        var response = await _client.GetAsync("/orders/1");
        response.EnsureSuccessStatusCode();

        var raw = await response.Content.ReadAsStringAsync();

        // Simple "format" checks: this asserts we get the expected JSON shape/casing.
        Assert.Contains("\"id\":", raw);
        Assert.Contains("\"productCode\":", raw);
        Assert.Contains("\"quantity\":", raw);
        Assert.Contains("\"createdAtUtc\":", raw);
    }
}