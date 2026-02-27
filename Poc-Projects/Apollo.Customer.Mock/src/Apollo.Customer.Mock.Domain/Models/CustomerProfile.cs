namespace Apollo.Customer.Mock.Domain.Models;

/// <summary>
/// Represents customer profile input used for create and update operations.
/// </summary>
public sealed class CustomerProfile
{
    /// <summary>
    /// Gets or sets the first name.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the last name.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the customer is subscribed.
    /// </summary>
    public bool IsSubscribed { get; set; }
}

