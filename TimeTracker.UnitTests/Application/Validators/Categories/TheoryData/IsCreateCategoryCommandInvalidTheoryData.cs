using TimeTracker.Application.UseCases.Categories.Handlers;

namespace TimeTracker.UnitTests.Application.Validators.Categories.TheoryData;

public class IsCreateCategoryCommandInvalidTheoryData : TheoryData<CreateCategoryHandler.Command, string>
{
    public IsCreateCategoryCommandInvalidTheoryData()
    {
        // Empty name
        Add(new CreateCategoryHandler.Command(string.Empty, "Valid description", Guid.NewGuid()),
            "Category name is required.");

        // Null name
        Add(new CreateCategoryHandler.Command(null!, "Valid description", Guid.NewGuid()),
            "Category name is required.");

        // Whitespace only name
        Add(new CreateCategoryHandler.Command("   ", "Valid description", Guid.NewGuid()),
            "Category name is required.");

        // Name too long (201 characters)
        Add(new CreateCategoryHandler.Command(new string('a', 201), "Valid description", Guid.NewGuid()),
            "Category name cannot exceed 200 characters.");

        // Description too long (1001 characters)
        Add(new CreateCategoryHandler.Command("Valid name", new string('a', 1001), Guid.NewGuid()),
            "Category description cannot exceed 1000 characters.");

        // Organization does not exist
        Add(new CreateCategoryHandler.Command("Valid name", "Valid description", Guid.NewGuid()),
            "Organization does not exist.");
    }
}