using ChecklistPoc.Api.Domain.Orders;

namespace ChecklistPoc.Api.Repositories;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(OrderId id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Order>> ListAsync(CancellationToken cancellationToken);
    Task<bool> TryAddAsync(Order order, CancellationToken cancellationToken);
    Task<bool> TryUpdateAsync(Order order, CancellationToken cancellationToken);
}

