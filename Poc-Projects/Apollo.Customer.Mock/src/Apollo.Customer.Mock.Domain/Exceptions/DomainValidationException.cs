namespace Apollo.Customer.Mock.Domain.Exceptions;

/// <summary>
/// Represents a domain validation error.
/// </summary>
public sealed class DomainValidationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DomainValidationException"/> class.
    /// </summary>
    /// <param name="message">Validation error message.</param>
    public DomainValidationException(string message)
        : base(message)
    {
    }
}
             
