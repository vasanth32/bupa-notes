namespace Demo.OrderApi.Models;

public sealed record Order(
    int Id,
    string ProductCode,
    int Quantity,
    DateTimeOffset CreatedAtUtc
);

