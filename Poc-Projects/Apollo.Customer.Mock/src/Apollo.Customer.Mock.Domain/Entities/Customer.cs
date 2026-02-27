using System.Net.Mail;
using Apollo.Customer.Mock.Domain.Exceptions;

namespace Apollo.Customer.Mock.Domain.Entities;

/// <summary>
/// Represents a customer in the domain.
/// </summary>
public sealed class Customer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Customer"/> class for EF Core.
    /// </summary>
    private Customer()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Customer"/> class.
    /// </summary>
    /// <param name="firstName">First name.</param>
    /// <param name="lastName">Last name.</param>
    /// <param name="email">Email address.</param>
    /// <param name="isSubscribed">Subscription status.</param>
    public Customer(string firstName, string lastName, string email, bool isSubscribed)
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;

        FirstName = firstName ?? string.Empty;
        LastName = lastName ?? string.Empty;
        Email = email ?? string.Empty;
        IsSubscribed = isSubscribed;

        if (string.IsNullOrWhiteSpace(FirstName))
        {
            throw new DomainValidationException("FirstName is required.");
        }

        if (string.IsNullOrWhiteSpace(LastName))
        {
            throw new DomainValidationException("LastName is required.");
        }

        if (string.IsNullOrWhiteSpace(Email))
        {
            throw new DomainValidationException("Email is required.");
        }

        try
        {
            _ = new MailAddress(Email);
        }
        catch (FormatException)
        {
            throw new DomainValidationException("Email is invalid.");
        }
    }

    /// <summary>
    /// Gets the customer identifier.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the first name.
    /// </summary>
    public string FirstName { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the last name.
    /// </summary>
    public string LastName { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the email address.
    /// </summary>
    public string Email { get; private set; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether the customer is subscribed.
    /// </summary>
    public bool IsSubscribed { get; private set; }

    /// <summary>
    /// Gets the UTC creation timestamp.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Updates core customer fields.
    /// </summary>
    /// <param name="firstName">First name.</param>
    /// <param name="lastName">Last name.</param>
    /// <param name="email">Email.</param>
    /// <param name="isSubscribed">Subscription status.</param>
    public void Update(string firstName, string lastName, string email, bool isSubscribed)
    {
        FirstName = firstName ?? string.Empty;
        LastName = lastName ?? string.Empty;
        Email = email ?? string.Empty;
        IsSubscribed = isSubscribed;

        if (string.IsNullOrWhiteSpace(FirstName))
        {
            throw new DomainValidationException("FirstName is required.");
        }

        if (string.IsNullOrWhiteSpace(LastName))
        {
            throw new DomainValidationException("LastName is required.");
        }

        if (string.IsNullOrWhiteSpace(Email))
        {
            throw new DomainValidationException("Email is required.");
        }

        try
        {
            _ = new MailAddress(Email);
        }
        catch (FormatException)
        {
            throw new DomainValidationException("Email is invalid.");
        }
    }
}

