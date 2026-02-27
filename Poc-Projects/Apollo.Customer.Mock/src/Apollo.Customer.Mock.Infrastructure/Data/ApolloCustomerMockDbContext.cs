using CustomerEntity = Apollo.Customer.Mock.Domain.Entities.Customer;
using Microsoft.EntityFrameworkCore;

namespace Apollo.Customer.Mock.Infrastructure.Data;

/// <summary>
/// EF Core database context for the Apollo Customer Mock application.
/// </summary>
public sealed class ApolloCustomerMockDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApolloCustomerMockDbContext"/> class.
    /// </summary>
    /// <param name="options">DbContext options.</param>
    public ApolloCustomerMockDbContext(DbContextOptions<ApolloCustomerMockDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets customers.
    /// </summary>
    public DbSet<CustomerEntity> Customers { get; set; } = default!;

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var customer = modelBuilder.Entity<CustomerEntity>();

        customer.ToTable("Customers");
        customer.HasKey(x => x.Id);

        customer.Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        customer.Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(100);

        customer.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(320);

        customer.HasIndex(x => x.Email)
            .IsUnique();

        customer.Property(x => x.IsSubscribed)
            .IsRequired();

        customer.Property(x => x.CreatedAt)
            .IsRequired();
    }
}

