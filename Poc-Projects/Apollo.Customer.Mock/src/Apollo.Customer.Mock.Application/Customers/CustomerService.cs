using Apollo.Customer.Mock.Application.Common;
using Apollo.Customer.Mock.Application.Exceptions;
using CustomerEntity = Apollo.Customer.Mock.Domain.Entities.Customer;
using Apollo.Customer.Mock.Domain.Models;

namespace Apollo.Customer.Mock.Application.Customers;

/// <summary>
/// Implements customer application operations.
/// </summary>
public sealed class CustomerService : ServiceBase, ICustomerService
{
    private readonly ICustomerRepository _customerRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomerService"/> class.
    /// </summary>
    /// <param name="customerRepository">Customer repository.</param>
    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    /// <inheritdoc />
    public Task<IEnumerable<CustomerEntity>> GetCustomersAsync(PaginationRequest pagination, CancellationToken cancellationToken)
    {
        return _customerRepository.GetPagedAsync(pagination, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<CustomerEntity> GetCustomerByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _customerRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new CustomerNotFoundException(id);
    }

    /// <inheritdoc />
    public async Task<CustomerEntity> CreateCustomerAsync(CustomerProfile profile, CancellationToken cancellationToken)
    {
        var customer = new CustomerEntity(profile.FirstName, profile.LastName, profile.Email, profile.IsSubscribed);

        return await _customerRepository.AddAsync(customer, cancellationToken)
            ?? throw new CustomerPersistenceException("Customer could not be created.");
    }

    /// <inheritdoc />
    public async Task<CustomerEntity> UpdateCustomerAsync(Guid id, CustomerProfile profile, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new CustomerNotFoundException(id);

        customer.Update(profile.FirstName, profile.LastName, profile.Email, profile.IsSubscribed);

        var updated = await _customerRepository.UpdateAsync(customer, cancellationToken);
        if (!updated)
        {
            throw new CustomerPersistenceException("Customer could not be updated.");
        }

        return customer;
    }

    /// <inheritdoc />
    public async Task<CustomerEntity> UpdateMarketingPreferenceAsync(
        Guid id,
        MarketingPreference preference,
        CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new CustomerNotFoundException(id);

        customer.Update(customer.FirstName, customer.LastName, customer.Email, preference.IsSubscribed);

        var updated = await _customerRepository.UpdateAsync(customer, cancellationToken);
        if (!updated)
        {
            throw new CustomerPersistenceException("Marketing preference could not be updated.");
        }

        return customer;
    }

    /// <inheritdoc />
    public async Task DeleteCustomerAsync(Guid id, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new CustomerNotFoundException(id);

        var deleted = await _customerRepository.DeleteAsync(customer, cancellationToken);
        if (!deleted)
        {
            throw new CustomerPersistenceException("Customer could not be deleted.");
        }
    }
}

