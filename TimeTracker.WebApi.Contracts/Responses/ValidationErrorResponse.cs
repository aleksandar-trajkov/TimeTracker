namespace TimeTracker.WebApi.Contracts.Responses;

public record ValidationErrorResponse
{
    /// <summary>
    /// Property for which the validation failed
    /// </summary>
    public required string Property { get; set; } = null!;

    /// <summary>
    /// Description of what failed
    /// </summary>
    public required string Message { get; set; } = null!;
}
