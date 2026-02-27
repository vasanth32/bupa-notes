using Apollo.Customer.Mock.Application.Customers;
using CustomerEntity = global::Apollo.Customer.Mock.Domain.Entities.Customer;
using Apollo.Customer.Mock.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Apollo.Customer.Mock.WebApi.Controllers;

/// <summary>
/// Customer API endpoints.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomersController"/> class.
    /// </summary>
    /// <param name="customerService">Customer service.</param>
    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    /// <summary>
    /// Gets customers (paged).
    /// </summary>
    /// <param name="pagination">Pagination input.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Customers.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CustomerEntity>>> GetAsync(
        [FromQuery] PaginationRequest pagination,
        CancellationToken cancellationToken)
    {
        var customers = await _customerService.GetCustomersAsync(pagination, cancellationToken);
        return Ok(customers);
    }

    /// <summary>
    /// Gets a customer by identifier.
    /// </summary>
    /// <param name="id">Customer identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Customer.</returns>
    [HttpGet("{id:guid}", Name = "GetCustomerById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerEntity>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var customer = await _customerService.GetCustomerByIdAsync(id, cancellationToken);
        return Ok(customer);
    }

    /// <summary>
    /// Creates a new customer.
    /// </summary>
    /// <param name="profile">Customer profile.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created customer.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CustomerEntity>> PostAsync(
        [FromBody] CustomerProfile profile,
        CancellationToken cancellationToken)
    {
        var customer = await _customerService.CreateCustomerAsync(profile, cancellationToken);
        return CreatedAtRoute("GetCustomerById", new { id = customer.Id }, customer);
    }

    /// <summary>
    /// Updates a customer.
    /// </summary>
    /// <param name="id">Customer identifier.</param>
    /// <param name="profile">Customer profile.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Updated customer.</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CustomerEntity>> PutAsync(
        Guid id,
        [FromBody] CustomerProfile profile,
        CancellationToken cancellationToken)
    {
        var customer = await _customerService.UpdateCustomerAsync(id, profile, cancellationToken);
        return Ok(customer);
    }

    /// <summary>
    /// Updates marketing preference only.
    /// </summary>
    /// <param name="id">Customer identifier.</param>
    /// <param name="preference">Marketing preference.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Updated customer.</returns>
    [HttpPut("{id:guid}/marketing-preference")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CustomerEntity>> PutMarketingPreferenceAsync(
        Guid id,
        [FromBody] MarketingPreference preference,
        CancellationToken cancellationToken)
    {
        var customer = await _customerService.UpdateMarketingPreferenceAsync(id, preference, cancellationToken);
        return Ok(customer);
    }

    /// <summary>
    /// Deletes a customer.
    /// </summary>
    /// <param name="id">Customer identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content.</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        await _customerService.DeleteCustomerAsync(id, cancellationToken);

        return NoContent();
    }
}

