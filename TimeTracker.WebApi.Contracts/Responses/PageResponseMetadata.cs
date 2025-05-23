namespace TimeTracker.WebApi.Contracts.Responses;

/// <summary>
/// Details for paged results
/// </summary>
public record PageResponseMetadata
{
    /// <summary>
    /// Current requested page number
    /// </summary>
    public int PageNumber { get; init; }

    /// <summary>
    /// Requested page size
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// Total items based on current request criteria
    /// </summary>
    public int TotalItems { get; init; }
}

