using TimeTracker.Application.UseCases.Categories.Handlers;
using TimeTracker.UnitTests.Common.Builders.Application.Categories;

namespace TimeTracker.UnitTests.Application.Validators.Categories.TheoryData;

public class UpdateCategoryValidatorInvalidTheoryData : TheoryData<UpdateCategoryHandler.Command, string>
{
    public UpdateCategoryValidatorInvalidTheoryData()
    {
        // Empty ID
        Add(new UpdateCategoryCommandBuilder().WithId(Guid.Empty).WithName("Valid name").WithDescription("Valid description").Build(),
            "Category ID is required.");

        // Category does not exist
        Add(new UpdateCategoryCommandBuilder().WithId(Guid.NewGuid()).WithName("Valid name").WithDescription("Valid description").Build(),
            "Category does not exist.");

        // Empty name
        Add(new UpdateCategoryCommandBuilder().WithId(Guid.NewGuid()).WithName(string.Empty).WithDescription("Valid description").Build(),
            "Category name is required.");

        // Null name
        Add(new UpdateCategoryCommandBuilder().WithId(Guid.NewGuid()).WithName(null!).WithDescription("Valid description").Build(),
            "Category name is required.");

        // Whitespace only name
        Add(new UpdateCategoryCommandBuilder().WithId(Guid.NewGuid()).WithName("   ").WithDescription("Valid description").Build(),
            "Category name is required.");

        // Name too long (201 characters)
        Add(new UpdateCategoryCommandBuilder().WithId(Guid.NewGuid()).WithName(new string('a', 201)).WithDescription("Valid description").Build(),
            "Category name cannot exceed 200 characters.");

        // Description too long (1001 characters)
        Add(new UpdateCategoryCommandBuilder().WithId(Guid.NewGuid()).WithName("Valid name").WithDescription(new string('a', 1001)).Build(),
            "Category description cannot exceed 1000 characters.");
    }
}
