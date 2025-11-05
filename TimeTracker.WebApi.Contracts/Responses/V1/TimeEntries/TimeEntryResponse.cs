namespace TimeTracker.WebApi.Contracts.Responses.V1.TimeEntries;

public class TimeEntryResponse
{
    public DateTimeOffset From { get; set; }
    public DateTimeOffset To { get; set; }
    public string Description { get; set; } = string.Empty;

    public CategoryResponse Category { get; set; } = null!;
}