using CustomerEntity = Apollo.Customer.Mock.Domain.Entities.Customer;
using Apollo.Customer.Mock.Domain.Models;

namespace Apollo.Customer.Mock.Application.Customers;

/// <summary>
/// Defines customer persistence operations.
/// </summary>
public interface ICustomerRepository
{
    /// <summary>
    /// Gets all customers.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>All customers.</returns>
    Task<IEnumerable<CustomerEntity>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Gets a page of customers.
    /// </summary>
    /// <param name="pagination">Pagination input.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Customers page.</returns>
    Task<IEnumerable<CustomerEntity>> GetPagedAsync(PaginationRequest pagination, CancellationToken cancellationToken);

    /// <summary>
    /// Gets a customer by identifier.
    /// </summary>
    /// <param name="id">Customer identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Customer or null.</returns>
    Task<CustomerEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Adds a new customer.
    /// </summary>
    /// <param name="customer">Customer.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created customer or null.</returns>
    Task<CustomerEntity?> AddAsync(CustomerEntity customer, CancellationToken cancellationToken);

    /// <summary>
    /// Updates an existing customer.
    /// </summary>
    /// <param name="customer">Customer.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if persisted; otherwise false.</returns>
    Task<bool> UpdateAsync(CustomerEntity customer, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes a customer.
    /// </summary>
    /// <param name="customer">Customer.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if deleted; otherwise false.</returns>
    Task<bool> DeleteAsync(CustomerEntity customer, CancellationToken cancellationToken);
}

