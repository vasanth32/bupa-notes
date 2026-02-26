using ChecklistPoc.Api.Domain.Orders;

namespace ChecklistPoc.Api.Exceptions;

public sealed class OrderConflictException : ChecklistPocException
{
    public OrderConflictException(OrderId orderId, string reason)
        : base($"Order '{orderId.Value}' has a conflict: {reason}")
    {
    }
}

