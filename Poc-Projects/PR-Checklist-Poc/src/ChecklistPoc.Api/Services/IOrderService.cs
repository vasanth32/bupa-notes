using ChecklistPoc.Api.Domain.Orders;

namespace ChecklistPoc.Api.Services;

public interface IOrderService
{
    Task<Order> CreateOrderAsync(OrderDraft draft, CancellationToken cancellationToken);
    Task<Order> GetOrderAsync(OrderId orderId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Order>> ListOrdersAsync(CancellationToken cancellationToken);
    Task<Order> CancelOrderAsync(OrderId orderId, CancellationToken cancellationToken);
}

