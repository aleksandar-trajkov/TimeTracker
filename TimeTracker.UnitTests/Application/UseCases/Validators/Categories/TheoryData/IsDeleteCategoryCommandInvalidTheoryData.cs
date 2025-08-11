using TimeTracker.Application.UseCases.Categories.Handlers;

namespace TimeTracker.UnitTests.Application.UseCases.Validators.Categories.TheoryData;

public class IsDeleteCategoryCommandInvalidTheoryData : TheoryData<DeleteCategoryHandler.Command, string>
{
    public IsDeleteCategoryCommandInvalidTheoryData()
    {
        // Empty ID
        Add(new DeleteCategoryHandler.Command(Guid.Empty),
            "Category ID is required.");

        // Category does not exist
        Add(new DeleteCategoryHandler.Command(Guid.NewGuid()),
            "Category does not exist.");
    }
}