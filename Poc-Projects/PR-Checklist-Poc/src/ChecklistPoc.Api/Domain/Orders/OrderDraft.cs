namespace ChecklistPoc.Api.Domain.Orders;

public sealed record OrderDraft(string CustomerId, IReadOnlyList<OrderLine> Lines);

