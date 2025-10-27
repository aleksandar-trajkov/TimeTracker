using TimeTracker.Application.UseCases.Categories.Handlers;

namespace TimeTracker.UnitTests.Application.Validators.Categories.TheoryData;

public class ValidUpdateCategoryCommandTheoryData : TheoryData<UpdateCategoryHandler.Command>
{
    public ValidUpdateCategoryCommandTheoryData()
    {
        // Valid command with description
        Add(new UpdateCategoryHandler.Command(Guid.NewGuid(), "Valid Category Name", "Valid description"));
        
        // Valid command with null description
        Add(new UpdateCategoryHandler.Command(Guid.NewGuid(), "Valid Category Name", null));

        // Valid command with maximum length name (200 characters)
        Add(new UpdateCategoryHandler.Command(Guid.NewGuid(), new string('a', 200), "Valid description"));

        // Valid command with maximum length description (1000 characters)
        Add(new UpdateCategoryHandler.Command(Guid.NewGuid(), "Valid name", new string('a', 1000)));

        // Valid command with empty description
        Add(new UpdateCategoryHandler.Command(Guid.NewGuid(), "Valid name", ""));

        // Valid command with whitespace only description (should pass as it's only validated when not null/empty)
        Add(new UpdateCategoryHandler.Command(Guid.NewGuid(), "Valid name", "   "));

        // Valid command with single character name
        Add(new UpdateCategoryHandler.Command(Guid.NewGuid(), "A", "Valid description"));
    }
}