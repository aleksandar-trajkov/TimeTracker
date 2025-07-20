namespace TimeTracker.WebApi.Contracts.Responses.TimeEntries;

public class CategoriesResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}