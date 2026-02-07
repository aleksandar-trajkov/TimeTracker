namespace TimeTracker.WebApi.Contracts.Requests.V1.TimeEntries;

public class CreateTimeEntryRequest
{
    public DateTimeOffset From { get; set; }
    public DateTimeOffset To { get; set; }
    public string Description { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
}