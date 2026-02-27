using Apollo.Customer.Mock.Application.Common;
using CustomerEntity = Apollo.Customer.Mock.Domain.Entities.Customer;
using Apollo.Customer.Mock.Domain.Models;

namespace Apollo.Customer.Mock.Application.Customers;

/// <summary>
/// Defines customer application operations.
/// </summary>
public interface ICustomerService : IService
{
    /// <summary>
    /// Gets customers (paged).
    /// </summary>
    /// <param name="pagination">Pagination input.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Customers.</returns>
    Task<IEnumerable<CustomerEntity>> GetCustomersAsync(PaginationRequest pagination, CancellationToken cancellationToken);

    /// <summary>
    /// Gets a customer by identifier.
    /// </summary>
    /// <param name="id">Customer identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Customer.</returns>
    Task<CustomerEntity> GetCustomerByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Creates a new customer.
    /// </summary>
    /// <param name="profile">Customer profile.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created customer.</returns>
    Task<CustomerEntity> CreateCustomerAsync(CustomerProfile profile, CancellationToken cancellationToken);

    /// <summary>
    /// Updates an existing customer.
    /// </summary>
    /// <param name="id">Customer identifier.</param>
    /// <param name="profile">Customer profile.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Updated customer.</returns>
    Task<CustomerEntity> UpdateCustomerAsync(Guid id, CustomerProfile profile, CancellationToken cancellationToken);

    /// <summary>
    /// Updates marketing preference only.
    /// </summary>
    /// <param name="id">Customer identifier.</param>
    /// <param name="preference">Marketing preference.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Updated customer.</returns>
    Task<CustomerEntity> UpdateMarketingPreferenceAsync(Guid id, MarketingPreference preference, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes a customer.
    /// </summary>
    /// <param name="id">Customer identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Task.</returns>
    Task DeleteCustomerAsync(Guid id, CancellationToken cancellationToken);
}

