using Apollo.Customer.Mock.Application.Customers;
using CustomerEntity = Apollo.Customer.Mock.Domain.Entities.Customer;
using Apollo.Customer.Mock.Domain.Models;
using Apollo.Customer.Mock.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Apollo.Customer.Mock.Infrastructure.Repositories;

/// <summary>
/// Implements customer persistence using EF Core.
/// </summary>
public sealed class CustomerRepository : ICustomerRepository
{
    private readonly ApolloCustomerMockDbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomerRepository"/> class.
    /// </summary>
    /// <param name="dbContext">DbContext.</param>
    public CustomerRepository(ApolloCustomerMockDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<CustomerEntity>> GetAllAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await _dbContext.Customers
                .AsNoTracking()
                .OrderBy(x => x.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        catch (InvalidOperationException)
        {
            return Array.Empty<CustomerEntity>();
        }
        catch (SqlException)
        {
            return Array.Empty<CustomerEntity>();
        }
        catch (DbUpdateConcurrencyException)
        {
            return Array.Empty<CustomerEntity>();
        }
        catch (DbUpdateException)
        {
            return Array.Empty<CustomerEntity>();
        }
        catch (Exception)
        {
            return Array.Empty<CustomerEntity>();
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<CustomerEntity>> GetPagedAsync(PaginationRequest pagination, CancellationToken cancellationToken)
    {
        try
        {
            return await _dbContext.Customers
                .AsNoTracking()
                .OrderBy(x => x.CreatedAt)
                .Skip(pagination.Skip)
                .Take(pagination.Count)
                .ToListAsync(cancellationToken);
        }
        catch (InvalidOperationException)
        {
            return Array.Empty<CustomerEntity>();
        }
        catch (SqlException)
        {
            return Array.Empty<CustomerEntity>();
        }
        catch (DbUpdateConcurrencyException)
        {
            return Array.Empty<CustomerEntity>();
        }
        catch (DbUpdateException)
        {
            return Array.Empty<CustomerEntity>();
        }
        catch (Exception)
        {
            return Array.Empty<CustomerEntity>();
        }
    }

    /// <inheritdoc />
    public async Task<CustomerEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            return await _dbContext.Customers
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }
        catch (InvalidOperationException)
        {
            return null;
        }
        catch (SqlException)
        {
            return null;
        }
        catch (DbUpdateConcurrencyException)
        {
            return null;
        }
        catch (DbUpdateException)
        {
            return null;
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<CustomerEntity?> AddAsync(CustomerEntity customer, CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.Customers.AddAsync(customer, cancellationToken);
            _ = await _dbContext.SaveChangesAsync(cancellationToken);
            return customer;
        }
        catch (InvalidOperationException)
        {
            return null;
        }
        catch (SqlException)
        {
            return null;
        }
        catch (DbUpdateConcurrencyException)
        {
            return null;
        }
        catch (DbUpdateException)
        {
            return null;
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<bool> UpdateAsync(CustomerEntity customer, CancellationToken cancellationToken)
    {
        try
        {
            _dbContext.Customers.Update(customer);
            _ = await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
        catch (SqlException)
        {
            return false;
        }
        catch (DbUpdateConcurrencyException)
        {
            return false;
        }
        catch (DbUpdateException)
        {
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(CustomerEntity customer, CancellationToken cancellationToken)
    {
        try
        {
            _dbContext.Customers.Remove(customer);
            _ = await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
        catch (SqlException)
        {
            return false;
        }
        catch (DbUpdateConcurrencyException)
        {
            return false;
        }
        catch (DbUpdateException)
        {
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }
}

