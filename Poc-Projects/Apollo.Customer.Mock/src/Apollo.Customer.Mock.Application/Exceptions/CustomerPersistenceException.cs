namespace Apollo.Customer.Mock.Application.Exceptions;

/// <summary>
/// Represents a persistence failure during a customer operation.
/// </summary>
public sealed class CustomerPersistenceException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CustomerPersistenceException"/> class.
    /// </summary>
    /// <param name="message">Error message.</param>
    public CustomerPersistenceException(string message)
        : base(message)
    {
    }
}

