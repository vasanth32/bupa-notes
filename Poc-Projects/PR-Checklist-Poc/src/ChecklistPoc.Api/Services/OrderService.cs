using ChecklistPoc.Api.Domain.Orders;
using ChecklistPoc.Api.Exceptions;
using ChecklistPoc.Api.Repositories;
using ChecklistPoc.Api.Validation;

namespace ChecklistPoc.Api.Services;

public sealed class OrderService(IOrderRepository orderRepository) : IOrderService
{
    public async Task<Order> CreateOrderAsync(OrderDraft draft, CancellationToken cancellationToken)
    {
        var errors = OrderDraftValidator.Validate(draft);
        if (errors.Count > 0)
        {
            throw new ValidationException("OrderDraft is invalid.", errors);
        }

        var order = new Order(
            Id: OrderId.New(),
            CustomerId: draft.CustomerId.Trim(),
            Status: OrderStatus.New,
            Lines: draft.Lines.ToArray(),
            CreatedAtUtc: DateTimeOffset.UtcNow);

        var added = await orderRepository.TryAddAsync(order, cancellationToken);
        if (!added)
        {
            throw new OrderConflictException(order.Id, "Unable to persist the new order.");
        }

        return order;
    }

    public async Task<Order> GetOrderAsync(OrderId orderId, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order is null)
        {
            throw new OrderNotFoundException(orderId);
        }

        return order;
    }

    public Task<IReadOnlyList<Order>> ListOrdersAsync(CancellationToken cancellationToken)
        => orderRepository.ListAsync(cancellationToken);

    public async Task<Order> CancelOrderAsync(OrderId orderId, CancellationToken cancellationToken)
    {
        var existing = await orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (existing is null)
        {
            throw new OrderNotFoundException(orderId);
        }

        if (existing.Status == OrderStatus.Cancelled)
        {
            throw new OrderConflictException(orderId, "Order is already cancelled.");
        }

        var updated = existing with { Status = OrderStatus.Cancelled };
        var saved = await orderRepository.TryUpdateAsync(updated, cancellationToken);
        if (!saved)
        {
            throw new OrderConflictException(orderId, "Unable to persist the cancellation.");
        }

        return updated;
    }
}

