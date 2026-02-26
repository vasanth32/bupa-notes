using ChecklistPoc.Api.Domain.Orders;
using ChecklistPoc.Api.Exceptions;
using ChecklistPoc.Api.Repositories;
using ChecklistPoc.Api.Services;
using Moq;

namespace ChecklistPoc.UnitTests.Services;

public sealed class OrderServiceTests
{
    [Fact]
    public async Task CreateOrderAsync_WhenDraftInvalid_ThrowsAndDoesNotCallRepository()
    {
        var repo = new Mock<IOrderRepository>(MockBehavior.Strict);
        var sut = new OrderService(repo.Object);

        var invalid = new OrderDraft(CustomerId: "", Lines: []);

        var ex = await Assert.ThrowsAsync<ValidationException>(
            () => sut.CreateOrderAsync(invalid, cancellationToken: CancellationToken.None));

        Assert.Contains("invalid", ex.Message, StringComparison.OrdinalIgnoreCase);
        repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CreateOrderAsync_WhenValid_PersistsAndReturnsOrder()
    {
        var ct = new CancellationTokenSource().Token;

        var repo = new Mock<IOrderRepository>(MockBehavior.Strict);
        repo.Setup(r => r.TryAddAsync(It.IsAny<Order>(), ct))
            .ReturnsAsync(true);

        var sut = new OrderService(repo.Object);

        var draft = new OrderDraft(
            CustomerId: "CUST-1",
            Lines:
            [
                new OrderLine("P-1", Quantity: 2, UnitPrice: 10m),
                new OrderLine("P-2", Quantity: 1, UnitPrice: 5m),
            ]);

        var created = await sut.CreateOrderAsync(draft, ct);

        Assert.NotEqual(Guid.Empty, created.Id.Value);
        Assert.Equal("CUST-1", created.CustomerId);
        Assert.Equal(OrderStatus.New, created.Status);
        Assert.Equal(25m, created.Total);

        repo.Verify(r => r.TryAddAsync(It.Is<Order>(o =>
            o.Id == created.Id &&
            o.CustomerId == created.CustomerId &&
            o.Status == OrderStatus.New &&
            o.Lines.Count == 2), ct), Times.Once);
        repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CreateOrderAsync_WhenRepositoryCannotPersist_ThrowsConflict()
    {
        var ct = new CancellationTokenSource().Token;

        var repo = new Mock<IOrderRepository>(MockBehavior.Strict);
        repo.Setup(r => r.TryAddAsync(It.IsAny<Order>(), ct))
            .ReturnsAsync(false);

        var sut = new OrderService(repo.Object);

        var draft = new OrderDraft(
            CustomerId: "CUST-1",
            Lines: [new OrderLine("P-1", Quantity: 1, UnitPrice: 10m)]);

        await Assert.ThrowsAsync<OrderConflictException>(() => sut.CreateOrderAsync(draft, ct));

        repo.Verify(r => r.TryAddAsync(It.IsAny<Order>(), ct), Times.Once);
        repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetOrderAsync_WhenMissing_ThrowsNotFound()
    {
        var ct = new CancellationTokenSource().Token;
        var id = new OrderId(Guid.NewGuid());

        var repo = new Mock<IOrderRepository>(MockBehavior.Strict);
        repo.Setup(r => r.GetByIdAsync(id, ct))
            .ReturnsAsync((Order?)null);

        var sut = new OrderService(repo.Object);

        await Assert.ThrowsAsync<OrderNotFoundException>(() => sut.GetOrderAsync(id, ct));

        repo.VerifyAll();
        repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetOrderAsync_WhenFound_ReturnsOrder()
    {
        var ct = new CancellationTokenSource().Token;
        var id = new OrderId(Guid.NewGuid());
        var order = new Order(
            id,
            CustomerId: "CUST-1",
            Status: OrderStatus.New,
            Lines: [new OrderLine("P-1", 1, 10m)],
            CreatedAtUtc: DateTimeOffset.UtcNow);

        var repo = new Mock<IOrderRepository>(MockBehavior.Strict);
        repo.Setup(r => r.GetByIdAsync(id, ct))
            .ReturnsAsync(order);

        var sut = new OrderService(repo.Object);

        var actual = await sut.GetOrderAsync(id, ct);

        Assert.Equal(order, actual);
        repo.VerifyAll();
        repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ListOrdersAsync_DelegatesToRepository()
    {
        var ct = new CancellationTokenSource().Token;
        IReadOnlyList<Order> orders =
        [
            new Order(
                new OrderId(Guid.NewGuid()),
                CustomerId: "CUST-1",
                Status: OrderStatus.New,
                Lines: [new OrderLine("P-1", 1, 10m)],
                CreatedAtUtc: DateTimeOffset.UtcNow),
        ];

        var repo = new Mock<IOrderRepository>(MockBehavior.Strict);
        repo.Setup(r => r.ListAsync(ct))
            .ReturnsAsync(orders);

        var sut = new OrderService(repo.Object);

        var actual = await sut.ListOrdersAsync(ct);

        Assert.Same(orders, actual);
        repo.VerifyAll();
        repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CancelOrderAsync_WhenAlreadyCancelled_ThrowsConflictAndDoesNotUpdate()
    {
        var ct = new CancellationTokenSource().Token;
        var id = new OrderId(Guid.NewGuid());
        var existing = new Order(
            id,
            CustomerId: "CUST-1",
            Status: OrderStatus.Cancelled,
            Lines: [new OrderLine("P-1", 1, 10m)],
            CreatedAtUtc: DateTimeOffset.UtcNow);

        var repo = new Mock<IOrderRepository>(MockBehavior.Strict);
        repo.Setup(r => r.GetByIdAsync(id, ct))
            .ReturnsAsync(existing);

        var sut = new OrderService(repo.Object);

        await Assert.ThrowsAsync<OrderConflictException>(() => sut.CancelOrderAsync(id, ct));

        repo.VerifyAll();
        repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CancelOrderAsync_WhenMissing_ThrowsNotFound()
    {
        var ct = new CancellationTokenSource().Token;
        var id = new OrderId(Guid.NewGuid());

        var repo = new Mock<IOrderRepository>(MockBehavior.Strict);
        repo.Setup(r => r.GetByIdAsync(id, ct))
            .ReturnsAsync((Order?)null);

        var sut = new OrderService(repo.Object);

        await Assert.ThrowsAsync<OrderNotFoundException>(() => sut.CancelOrderAsync(id, ct));

        repo.VerifyAll();
        repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CancelOrderAsync_WhenNew_UpdatesAndReturnsCancelled()
    {
        var ct = new CancellationTokenSource().Token;
        var id = new OrderId(Guid.NewGuid());
        var existing = new Order(
            id,
            CustomerId: "CUST-1",
            Status: OrderStatus.New,
            Lines: [new OrderLine("P-1", 1, 10m)],
            CreatedAtUtc: DateTimeOffset.UtcNow);

        var repo = new Mock<IOrderRepository>(MockBehavior.Strict);
        repo.Setup(r => r.GetByIdAsync(id, ct))
            .ReturnsAsync(existing);
        repo.Setup(r => r.TryUpdateAsync(It.IsAny<Order>(), ct))
            .ReturnsAsync(true);

        var sut = new OrderService(repo.Object);

        var cancelled = await sut.CancelOrderAsync(id, ct);

        Assert.Equal(OrderStatus.Cancelled, cancelled.Status);

        repo.Verify(r => r.GetByIdAsync(id, ct), Times.Once);
        repo.Verify(r => r.TryUpdateAsync(It.Is<Order>(o =>
            o.Id == id && o.Status == OrderStatus.Cancelled), ct), Times.Once);
        repo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CancelOrderAsync_WhenUpdateFails_ThrowsConflict()
    {
        var ct = new CancellationTokenSource().Token;
        var id = new OrderId(Guid.NewGuid());
        var existing = new Order(
            id,
            CustomerId: "CUST-1",
            Status: OrderStatus.New,
            Lines: [new OrderLine("P-1", 1, 10m)],
            CreatedAtUtc: DateTimeOffset.UtcNow);

        var repo = new Mock<IOrderRepository>(MockBehavior.Strict);
        repo.Setup(r => r.GetByIdAsync(id, ct))
            .ReturnsAsync(existing);
        repo.Setup(r => r.TryUpdateAsync(It.IsAny<Order>(), ct))
            .ReturnsAsync(false);

        var sut = new OrderService(repo.Object);

        await Assert.ThrowsAsync<OrderConflictException>(() => sut.CancelOrderAsync(id, ct));

        repo.Verify(r => r.GetByIdAsync(id, ct), Times.Once);
        repo.Verify(r => r.TryUpdateAsync(It.IsAny<Order>(), ct), Times.Once);
        repo.VerifyNoOtherCalls();
    }
}

