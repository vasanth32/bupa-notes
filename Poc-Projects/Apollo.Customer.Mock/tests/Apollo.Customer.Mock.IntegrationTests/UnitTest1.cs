using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Apollo.Customer.Mock.Domain.Models;
using Apollo.Customer.Mock.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Apollo.Customer.Mock.IntegrationTests;

public sealed class UnitTest1
{
    [Fact]
    public async Task PostCustomerAsync_InvalidEmail_ReturnsBadRequest()
    {
        await using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/customers", new CustomerProfile
        {
            FirstName = "John",
            LastName = "Smith",
            Email = "invalid",
            IsSubscribed = true,
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetCustomerByIdAsync_WhenMissing_ReturnsNotFound()
    {
        await using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync($"/api/customers/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task CustomerLifecycle_WithPaginationAndMarketingPreference_Works()
    {
        await using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var create1 = await client.PostAsJsonAsync("/api/customers", new CustomerProfile
        {
            FirstName = "A",
            LastName = "A",
            Email = "a1@example.com",
            IsSubscribed = true,
        });
        create1.EnsureSuccessStatusCode();

        var created1 = await ReadCustomerAsync(create1);
        Assert.NotEqual(Guid.Empty, created1.Id);

        var create2 = await client.PostAsJsonAsync("/api/customers", new CustomerProfile
        {
            FirstName = "B",
            LastName = "B",
            Email = "b1@example.com",
            IsSubscribed = true,
        });
        create2.EnsureSuccessStatusCode();

        var list = await client.GetAsync("/api/customers?skip=1&count=1");
        list.EnsureSuccessStatusCode();

        var page = await list.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(JsonValueKind.Array, page.ValueKind);
        Assert.Equal(1, page.GetArrayLength());

        var pref = await client.PutAsJsonAsync(
            $"/api/customers/{created1.Id}/marketing-preference",
            new MarketingPreference { IsSubscribed = false });
        pref.EnsureSuccessStatusCode();

        var updated = await ReadCustomerAsync(pref);
        Assert.False(updated.IsSubscribed);
    }

    private static async Task<CustomerDto> ReadCustomerAsync(HttpResponseMessage response)
    {
        var customer = await response.Content.ReadFromJsonAsync<CustomerDto>();
        Assert.NotNull(customer);
        return customer!;
    }

    private sealed record CustomerDto(
        Guid Id,
        string FirstName,
        string LastName,
        string Email,
        bool IsSubscribed,
        DateTime CreatedAt);

    private sealed class TestWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly string _databaseName = $"ApolloCustomerMock-{Guid.NewGuid()}";

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Development");

            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<ApolloCustomerMockDbContext>));
                services.RemoveAll(typeof(ApolloCustomerMockDbContext));

                services.AddDbContext<ApolloCustomerMockDbContext>(options =>
                {
                    options.UseInMemoryDatabase(_databaseName);
                });
            });
        }
    }
}