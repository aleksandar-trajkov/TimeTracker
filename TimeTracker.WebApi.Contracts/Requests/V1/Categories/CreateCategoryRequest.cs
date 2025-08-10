namespace TimeTracker.WebApi.Contracts.Requests.V1.Categories;

public class CreateCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid OrganizationId { get; set; }
}