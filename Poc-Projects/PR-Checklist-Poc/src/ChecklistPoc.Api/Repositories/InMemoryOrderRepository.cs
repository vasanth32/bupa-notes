using System.Collections.Concurrent;
using ChecklistPoc.Api.Domain.Orders;

namespace ChecklistPoc.Api.Repositories;

public sealed class InMemoryOrderRepository : IOrderRepository
{
    private readonly ConcurrentDictionary<OrderId, Order> _store = new();

    public Task<Order?> GetByIdAsync(OrderId id, CancellationToken cancellationToken)
    {
        try
        {
            _store.TryGetValue(id, out var order);
            return Task.FromResult(order);
        }
        catch
        {
            return Task.FromResult<Order?>(null);
        }
    }

    public Task<IReadOnlyList<Order>> ListAsync(CancellationToken cancellationToken)
    {
        try
        {
            IReadOnlyList<Order> list = _store.Values
                .OrderByDescending(o => o.CreatedAtUtc)
                .ToArray();

            return Task.FromResult(list);
        }
        catch
        {
            return Task.FromResult<IReadOnlyList<Order>>([]);
        }
    }

    public Task<bool> TryAddAsync(Order order, CancellationToken cancellationToken)
    {
        try
        {
            return Task.FromResult(_store.TryAdd(order.Id, order));
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public Task<bool> TryUpdateAsync(Order order, CancellationToken cancellationToken)
    {
        try
        {
            if (!_store.TryGetValue(order.Id, out var existing))
            {
                return Task.FromResult(false);
            }

            return Task.FromResult(_store.TryUpdate(order.Id, order, existing));
        }
        catch
        {
            return Task.FromResult(false);
        }
    }
}

