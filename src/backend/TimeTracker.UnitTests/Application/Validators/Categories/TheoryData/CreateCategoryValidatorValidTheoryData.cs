using TimeTracker.Application.UseCases.Categories.Handlers;

namespace TimeTracker.UnitTests.Application.Validators.Categories.TheoryData;

public class CreateCategoryValidatorValidTheoryData : TheoryData<CreateCategoryHandler.Command>
{
    public CreateCategoryValidatorValidTheoryData()
    {
        // Valid command with organization ID
        Add(new CreateCategoryHandler.Command("Valid Category Name", "Valid description", Guid.NewGuid()));

        // Valid command with null description
        Add(new CreateCategoryHandler.Command("Valid Category Name", null, Guid.NewGuid()));

        // Valid command with empty organization ID (bypasses organization validation)
        Add(new CreateCategoryHandler.Command("Valid Category Name", "Valid description", Guid.Empty));

        // Valid command with null organization ID (bypasses organization validation)
        Add(new CreateCategoryHandler.Command("Valid Category Name", "Valid description", null));

        // Valid command with maximum length name (200 characters)
        Add(new CreateCategoryHandler.Command(new string('a', 200), "Valid description", Guid.NewGuid()));

        // Valid command with maximum length description (1000 characters)
        Add(new CreateCategoryHandler.Command("Valid name", new string('a', 1000), Guid.NewGuid()));

        // Valid command with empty description
        Add(new CreateCategoryHandler.Command("Valid name", "", Guid.NewGuid()));

        // Valid command with whitespace only description (should pass as it's only validated when not null/empty)
        Add(new CreateCategoryHandler.Command("Valid name", "   ", Guid.NewGuid()));
    }
}