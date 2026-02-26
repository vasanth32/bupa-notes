using ChecklistPoc.Api.Domain.Orders;

namespace ChecklistPoc.Api.Exceptions;

public sealed class OrderNotFoundException(OrderId orderId)
    : ChecklistPocException($"Order '{orderId.Value}' was not found.");

