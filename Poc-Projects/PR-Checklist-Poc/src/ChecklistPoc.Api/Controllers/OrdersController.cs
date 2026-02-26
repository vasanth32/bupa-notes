using ChecklistPoc.Api.Domain.Orders;
using ChecklistPoc.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChecklistPoc.Api.Controllers;

[ApiController]
[Route("orders")]
public sealed class OrdersController(IOrderService orderService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(Order), StatusCodes.Status201Created)]
    public async Task<ActionResult<Order>> CreateAsync([FromBody] OrderDraft draft, CancellationToken cancellationToken)
    {
        var created = await orderService.CreateOrderAsync(draft, cancellationToken);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = created.Id.Value }, created);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
    public async Task<ActionResult<Order>> GetByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var order = await orderService.GetOrderAsync(new OrderId(id), cancellationToken);
        return Ok(order);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<Order>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<Order>>> ListAsync(CancellationToken cancellationToken)
    {
        var orders = await orderService.ListOrdersAsync(cancellationToken);
        return Ok(orders);
    }

    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
    public async Task<ActionResult<Order>> CancelAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var updated = await orderService.CancelOrderAsync(new OrderId(id), cancellationToken);
        return Ok(updated);
    }
}

