namespace TimeTracker.WebApi.Contracts.Responses.V1.Categories;

public class CategoryResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}