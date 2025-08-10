namespace TimeTracker.WebApi.Contracts.Requests.V1.Categories;

public class UpdateCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}