namespace TimeTracker.WebApi.Contracts.Responses;

public record ServerErrorResponse
{
    /// <summary>
    /// Exception message
    /// </summary>
    public required string Message { get; set; } = null!;
}
