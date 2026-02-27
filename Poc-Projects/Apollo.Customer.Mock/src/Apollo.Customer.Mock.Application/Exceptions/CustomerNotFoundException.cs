namespace Apollo.Customer.Mock.Application.Exceptions;

/// <summary>
/// Represents a customer-not-found application error.
/// </summary>
public sealed class CustomerNotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CustomerNotFoundException"/> class.
    /// </summary>
    /// <param name="id">Customer identifier.</param>
    public CustomerNotFoundException(Guid id)
        : base($"Customer '{id}' was not found.")
    {
        Id = id;
    }

    /// <summary>
    /// Gets the missing customer identifier.
    /// </summary>
    public Guid Id { get; }
}

