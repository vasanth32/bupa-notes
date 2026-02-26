using Demo.OrderApi.Models;
using Demo.OrderApi.Repositories;
using Demo.OrderApi.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace Demo.OrderApi.UnitTests;

public sealed class OrderServiceTests
{
    [Fact]
    public async Task GetOrderById_ReturnsCorrectOrder()
    {
        var repo = new Mock<IOrderRepository>(MockBehavior.Strict);
        var logger = new Mock<ILogger<OrderService>>();
        var expected = new Order(Id: 10, ProductCode: "SKU-1", Quantity: 2, CreatedAtUtc: DateTimeOffset.UtcNow);

        repo.Setup(r => r.GetByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var sut = new OrderService(repo.Object, logger.Object);

        var actual = await sut.GetOrderByIdAsync(10, CancellationToken.None);

        Assert.Equal(expected, actual);
        repo.VerifyAll();
    }

    [Fact]
    public async Task GetOrderById_ThrowsIfNotFound()
    {
        var repo = new Mock<IOrderRepository>(MockBehavior.Strict);
        var logger = new Mock<ILogger<OrderService>>();

        repo.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        var sut = new OrderService(repo.Object, logger.Object);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.GetOrderByIdAsync(999, CancellationToken.None));
        repo.VerifyAll();
    }

    [Fact]
    public async Task GetOrderById_WhenRepositoryThrows_ServiceWrapsAndLogs()
    {
        var repo = new Mock<IOrderRepository>(MockBehavior.Strict);
        var logger = new Mock<ILogger<OrderService>>();
        var inner = new Exception("repo exploded");

        repo.Setup(r => r.GetByIdAsync(10, It.IsAny<CancellationToken>()))
            .ThrowsAsync(inner);

        var sut = new OrderService(repo.Object, logger.Object);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.GetOrderByIdAsync(10, CancellationToken.None));

        Assert.Same(inner, ex.InnerException);
        VerifyLogged(logger, LogLevel.Error, contains: "Repository failure while getting order.");
        repo.VerifyAll();
    }

    [Fact]
    public async Task CreateOrder_AddsNewOrder()
    {
        var repo = new Mock<IOrderRepository>(MockBehavior.Strict);
        var logger = new Mock<ILogger<OrderService>>();

        repo.Setup(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order o, CancellationToken _) => o with { Id = 123 });

        var sut = new OrderService(repo.Object, logger.Object);

        var created = await sut.CreateOrderAsync(new CreateOrderRequest(ProductCode: "SKU-NEW", Quantity: 3), CancellationToken.None);

        Assert.Equal(123, created.Id);
        Assert.Equal("SKU-NEW", created.ProductCode);
        Assert.Equal(3, created.Quantity);
        repo.VerifyAll();
    }

    [Fact]
    public async Task CreateOrder_LogsInformation()
    {
        var repo = new Mock<IOrderRepository>(MockBehavior.Strict);
        var logger = new Mock<ILogger<OrderService>>();

        repo.Setup(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order o, CancellationToken _) => o with { Id = 7 });

        var sut = new OrderService(repo.Object, logger.Object);

        await sut.CreateOrderAsync(new CreateOrderRequest(ProductCode: "SKU-LOG", Quantity: 1), CancellationToken.None);

        VerifyLogged(logger, LogLevel.Information, contains: "Created order.");
        repo.VerifyAll();
    }

    [Fact]
    public async Task RepositoryThrows_ServiceHandlesIt()
    {
        var repo = new Mock<IOrderRepository>(MockBehavior.Strict);
        var logger = new Mock<ILogger<OrderService>>();
        var inner = new Exception("db down");

        repo.Setup(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(inner);
            

        var sut = new OrderService(repo.Object, logger.Object);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            sut.CreateOrderAsync(new CreateOrderRequest(ProductCode: "SKU-FAIL", Quantity: 1), CancellationToken.None));

        Assert.Same(inner, ex.InnerException);
        VerifyLogged(logger, LogLevel.Error, contains: "Repository failure while creating order.");
        repo.VerifyAll();
    }

    // Moq-friendly verification for ILogger<T>.Log(...)
    private static void VerifyLogged(Mock<ILogger<OrderService>> logger, LogLevel level, string contains)
    {
        logger.Verify(
            x => x.Log(
                level,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!.Contains(contains)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }
}