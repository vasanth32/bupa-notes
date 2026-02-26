namespace Demo.OrderApi.Models;

public sealed record CreateOrderRequest(
    string ProductCode,
    int Quantity
);

