using System.Collections.Concurrent;
using Demo.OrderApi.Models;

namespace Demo.OrderApi.Repositories;

public sealed class FakeOrderRepository : IOrderRepository
{
    private readonly ConcurrentDictionary<int, Order> _orders = new();
    private int _nextId = 0;

    public FakeOrderRepository()
    {
        // Seed a couple of orders for quick GET testing.
        var now = DateTimeOffset.UtcNow;
        var o1 = new Order(Id: 1, ProductCode: "SKU-ABC", Quantity: 2, CreatedAtUtc: now);
        var o2 = new Order(Id: 2, ProductCode: "SKU-XYZ", Quantity: 1, CreatedAtUtc: now);
        _orders[o1.Id] = o1;
        _orders[o2.Id] = o2;
        _nextId = 2;
    }

    public Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _orders.TryGetValue(id, out var order);
        return Task.FromResult(order);
    }

    public Task<Order> AddAsync(Order order, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var id = Interlocked.Increment(ref _nextId);
        var created = order with { Id = id };
        _orders[id] = created;
        return Task.FromResult(created);
    }
}

