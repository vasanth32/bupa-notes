using System.ComponentModel.DataAnnotations;

namespace Apollo.Customer.Mock.Domain.Models;

/// <summary>
/// Represents pagination input for list endpoints.
/// </summary>
public sealed class PaginationRequest
{
    /// <summary>
    /// Gets or sets the number of records to skip.
    /// </summary>
    [Range(0, int.MaxValue)]
    public int Skip { get; set; }

    /// <summary>
    /// Gets or sets the number of records to take.
    /// </summary>
    [Range(1, 500)]
    public int Count { get; set; } = 50;
}

