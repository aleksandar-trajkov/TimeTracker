using TimeTracker.Application.UseCases.Categories.Handlers;

namespace TimeTracker.UnitTests.Application.Validators.Categories.TheoryData;

public class IsUpdateCategoryCommandInvalidTheoryData : TheoryData<UpdateCategoryHandler.Command, string>
{
    public IsUpdateCategoryCommandInvalidTheoryData()
    {
        // Empty ID
        Add(new UpdateCategoryHandler.Command(Guid.Empty, "Valid name", "Valid description"),
            "Category ID is required.");

        // Category does not exist
        Add(new UpdateCategoryHandler.Command(Guid.NewGuid(), "Valid name", "Valid description"),
            "Category does not exist.");

        // Empty name
        Add(new UpdateCategoryHandler.Command(Guid.NewGuid(), string.Empty, "Valid description"),
            "Category name is required.");

        // Null name
        Add(new UpdateCategoryHandler.Command(Guid.NewGuid(), null!, "Valid description"),
            "Category name is required.");

        // Whitespace only name
        Add(new UpdateCategoryHandler.Command(Guid.NewGuid(), "   ", "Valid description"),
            "Category name is required.");

        // Name too long (201 characters)
        Add(new UpdateCategoryHandler.Command(Guid.NewGuid(), new string('a', 201), "Valid description"),
            "Category name cannot exceed 200 characters.");

        // Description too long (1001 characters)
        Add(new UpdateCategoryHandler.Command(Guid.NewGuid(), "Valid name", new string('a', 1001)),
            "Category description cannot exceed 1000 characters.");
    }
}