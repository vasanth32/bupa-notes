using Demo.OrderApi.Models;
using Demo.OrderApi.Repositories;

namespace Demo.OrderApi.Services;

public sealed class OrderService
{
    private readonly IOrderRepository _repository;
    private readonly ILogger<OrderService> _logger;

    public OrderService(IOrderRepository repository, ILogger<OrderService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Order> GetOrderByIdAsync(int id, CancellationToken cancellationToken)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id), "Id must be positive.");

        try
        {
            var order = await _repository.GetByIdAsync(id, cancellationToken);
            if (order is null)
            {
                _logger.LogInformation("Order not found. Id={OrderId}", id);
                throw new KeyNotFoundException($"Order {id} not found.");
            }

            return order;
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Repository failure while getting order. Id={OrderId}", id);
            throw new InvalidOperationException("Repository failure while getting order.", ex);
        }
    }

    public async Task<Order> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        if (string.IsNullOrWhiteSpace(request.ProductCode)) throw new ArgumentException("ProductCode is required.", nameof(request));
        if (request.Quantity <= 0) throw new ArgumentOutOfRangeException(nameof(request.Quantity), "Quantity must be > 0.");

        var order = new Order(
            Id: 0,
            ProductCode: request.ProductCode.Trim(),
            Quantity: request.Quantity,
            CreatedAtUtc: DateTimeOffset.UtcNow
        );

        try
        {
            var created = await _repository.AddAsync(order, cancellationToken);
            _logger.LogInformation("Created order. Id={OrderId} ProductCode={ProductCode} Quantity={Quantity}", created.Id, created.ProductCode, created.Quantity);
            return created;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Repository failure while creating order. ProductCode={ProductCode}", order.ProductCode);
            throw new InvalidOperationException("Repository failure while creating order.", ex);
        }
    }
}

