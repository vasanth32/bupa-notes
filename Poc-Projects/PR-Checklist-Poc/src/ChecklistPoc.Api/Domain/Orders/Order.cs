namespace ChecklistPoc.Api.Domain.Orders;

public sealed record Order(
    OrderId Id,
    string CustomerId,
    OrderStatus Status,
    IReadOnlyList<OrderLine> Lines,
    DateTimeOffset CreatedAtUtc)
{
    public decimal Total => Lines.Sum(l => l.LineTotal);
}

