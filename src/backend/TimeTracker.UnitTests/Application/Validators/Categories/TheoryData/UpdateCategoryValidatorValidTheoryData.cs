using TimeTracker.Application.UseCases.Categories.Handlers;
using TimeTracker.UnitTests.Common.Builders.Application.Categories;

namespace TimeTracker.UnitTests.Application.Validators.Categories.TheoryData;

public class UpdateCategoryValidatorValidTheoryData : TheoryData<UpdateCategoryHandler.Command>
{
    public UpdateCategoryValidatorValidTheoryData()
    {
        // Valid command with description
        Add(new UpdateCategoryCommandBuilder().WithId(Guid.NewGuid()).WithName("Valid Category Name").WithDescription("Valid description").Build());

        // Valid command with null description
        Add(new UpdateCategoryCommandBuilder().WithId(Guid.NewGuid()).WithName("Valid Category Name").WithDescription(null).Build());

        // Valid command with maximum length name (200 characters)
        Add(new UpdateCategoryCommandBuilder().WithId(Guid.NewGuid()).WithName(new string('a', 200)).WithDescription("Valid description").Build());

        // Valid command with maximum length description (1000 characters)
        Add(new UpdateCategoryCommandBuilder().WithId(Guid.NewGuid()).WithName("Valid name").WithDescription(new string('a', 1000)).Build());

        // Valid command with empty description
        Add(new UpdateCategoryCommandBuilder().WithId(Guid.NewGuid()).WithName("Valid name").WithDescription("").Build());

        // Valid command with whitespace only description (should pass as it's only validated when not null/empty)
        Add(new UpdateCategoryCommandBuilder().WithId(Guid.NewGuid()).WithName("Valid name").WithDescription("   ").Build());

        // Valid command with single character name
        Add(new UpdateCategoryCommandBuilder().WithId(Guid.NewGuid()).WithName("A").WithDescription("Valid description").Build());
    }
}
