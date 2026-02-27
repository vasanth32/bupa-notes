using Apollo.Customer.Mock.Application.Customers;
using Apollo.Customer.Mock.Application.Exceptions;
using CustomerEntity = global::Apollo.Customer.Mock.Domain.Entities.Customer;
using Apollo.Customer.Mock.Domain.Exceptions;
using Apollo.Customer.Mock.Domain.Models;

namespace Apollo.Customer.Mock.UnitTests;

public sealed class UnitTest1
{
#region Create
    [Fact]
    public async Task CreateCustomerAsync_InvalidEmail_ThrowsDomainValidationException()
    {
        var repository = new InMemoryCustomerRepository();
        var service = new CustomerService(repository);

        var profile = new CustomerProfile
        {
            FirstName = "John",
            LastName = "Smith",
            Email = "not-an-email",
            IsSubscribed = true,
        };

        _ = await Assert.ThrowsAsync<DomainValidationException>(() =>
            service.CreateCustomerAsync(profile, CancellationToken.None));
    }

    [Fact]
    public async Task CreateCustomerAsync_RepositoryReturnsNull_ThrowsCustomerPersistenceException()
    {
        var repository = new InMemoryCustomerRepository { ReturnNullOnAdd = true };
        var service = new CustomerService(repository);

        var profile = new CustomerProfile
        {
            FirstName = "John",
            LastName = "Smith",
            Email = "john.smith@example.com",
            IsSubscribed = true,
        };

        _ = await Assert.ThrowsAsync<CustomerPersistenceException>(() =>
            service.CreateCustomerAsync(profile, CancellationToken.None));
    }
#endregion

#region List
    [Fact]
    public async Task GetCustomersAsync_WhenEmpty_ReturnsEmptyEnumerable()
    {
        var repository = new InMemoryCustomerRepository();
        var service = new CustomerService(repository);

        var customers = await service.GetCustomersAsync(new PaginationRequest { Skip = 0, Count = 50 }, CancellationToken.None);

        Assert.NotNull(customers);
        Assert.Empty(customers);
    }

    [Fact]
    public async Task GetCustomersAsync_WithPagination_ReturnsPage()
    {
        var repository = new InMemoryCustomerRepository();
        var service = new CustomerService(repository);

        _ = await service.CreateCustomerAsync(new CustomerProfile
        {
            FirstName = "A",
            LastName = "A",
            Email = "a@example.com",
            IsSubscribed = true,
        }, CancellationToken.None);

        _ = await service.CreateCustomerAsync(new CustomerProfile
        {
            FirstName = "B",
            LastName = "B",
            Email = "b@example.com",
            IsSubscribed = false,
        }, CancellationToken.None);

        var page = await service.GetCustomersAsync(new PaginationRequest { Skip = 1, Count = 1 }, CancellationToken.None);

        Assert.Single(page);
    }
#endregion

#region Update
    [Fact]
    public async Task UpdateCustomerAsync_WhenMissing_ThrowsCustomerNotFoundException()
    {
        var repository = new InMemoryCustomerRepository();
        var service = new CustomerService(repository);

        var profile = new CustomerProfile
        {
            FirstName = "Jane",
            LastName = "Doe",
            Email = "jane.doe@example.com",
            IsSubscribed = false,
        };

        _ = await Assert.ThrowsAsync<CustomerNotFoundException>(() =>
            service.UpdateCustomerAsync(Guid.NewGuid(), profile, CancellationToken.None));
    }
#endregion

#region MarketingPreference
    [Fact]
    public async Task UpdateMarketingPreferenceAsync_WhenMissing_ThrowsCustomerNotFoundException()
    {
        var repository = new InMemoryCustomerRepository();
        var service = new CustomerService(repository);

        _ = await Assert.ThrowsAsync<CustomerNotFoundException>(() =>
            service.UpdateMarketingPreferenceAsync(
                Guid.NewGuid(),
                new MarketingPreference { IsSubscribed = true },
                CancellationToken.None));
    }

    [Fact]
    public async Task UpdateMarketingPreferenceAsync_WhenUpdateFails_ThrowsCustomerPersistenceException()
    {
        var repository = new InMemoryCustomerRepository { ReturnFalseOnUpdate = true };
        var service = new CustomerService(repository);

        var created = await service.CreateCustomerAsync(new CustomerProfile
        {
            FirstName = "John",
            LastName = "Smith",
            Email = "john.smith2@example.com",
            IsSubscribed = true,
        }, CancellationToken.None);

        _ = await Assert.ThrowsAsync<CustomerPersistenceException>(() =>
            service.UpdateMarketingPreferenceAsync(
                created.Id,
                new MarketingPreference { IsSubscribed = false },
                CancellationToken.None));
    }

    [Fact]
    public async Task UpdateMarketingPreferenceAsync_Success_UpdatesIsSubscribed()
    {
        var repository = new InMemoryCustomerRepository();
        var service = new CustomerService(repository);

        var created = await service.CreateCustomerAsync(new CustomerProfile
        {
            FirstName = "John",
            LastName = "Smith",
            Email = "john.smith3@example.com",
            IsSubscribed = true,
        }, CancellationToken.None);

        var updated = await service.UpdateMarketingPreferenceAsync(
            created.Id,
            new MarketingPreference { IsSubscribed = false },
            CancellationToken.None);

        Assert.False(updated.IsSubscribed);
    }
#endregion

#region Delete
    [Fact]
    public async Task DeleteCustomerAsync_WhenMissing_ThrowsCustomerNotFoundException()
    {
        var repository = new InMemoryCustomerRepository();
        var service = new CustomerService(repository);

        _ = await Assert.ThrowsAsync<CustomerNotFoundException>(() =>
            service.DeleteCustomerAsync(Guid.NewGuid(), CancellationToken.None));
    }
#endregion

    private sealed class InMemoryCustomerRepository : ICustomerRepository
    {
        private readonly List<CustomerEntity> _customers = [];

        public bool ReturnNullOnAdd { get; set; }
        public bool ReturnFalseOnUpdate { get; set; }

        public Task<IEnumerable<CustomerEntity>> GetAllAsync(CancellationToken cancellationToken)
        {
            IEnumerable<CustomerEntity> result = _customers.ToList();
            return Task.FromResult(result);
        }

        public Task<IEnumerable<CustomerEntity>> GetPagedAsync(PaginationRequest pagination, CancellationToken cancellationToken)
        {
            IEnumerable<CustomerEntity> result = _customers
                .Skip(pagination.Skip)
                .Take(pagination.Count)
                .ToList();

            return Task.FromResult(result);
        }

        public Task<CustomerEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var customer = _customers.FirstOrDefault(x => x.Id == id);
            return Task.FromResult(customer);
        }

        public Task<CustomerEntity?> AddAsync(CustomerEntity customer, CancellationToken cancellationToken)
        {
            if (ReturnNullOnAdd)
            {
                return Task.FromResult<CustomerEntity?>(null);
            }

            _customers.Add(customer);
            return Task.FromResult<CustomerEntity?>(customer);
        }

        public Task<bool> UpdateAsync(CustomerEntity customer, CancellationToken cancellationToken)
        {
            if (ReturnFalseOnUpdate)
            {
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        public Task<bool> DeleteAsync(CustomerEntity customer, CancellationToken cancellationToken)
        {
            var removed = _customers.Remove(customer);
            return Task.FromResult(removed);
        }
    }
}