using Demo.OrderApi.Models;
using Demo.OrderApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace Demo.OrderApi.Controllers;

[ApiController]
[Route("orders")]
public sealed class OrderController : ControllerBase
{
    private readonly OrderService _service;

    public OrderController(OrderService service)
    {
        _service = service;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Order>> GetById([FromRoute] int id, CancellationToken cancellationToken)
    {
        var order = await _service.GetOrderByIdAsync(id, cancellationToken);
        return Ok(order);
    }

    [HttpPost]
    public async Task<ActionResult<Order>> Create([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var created = await _service.CreateOrderAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
}

