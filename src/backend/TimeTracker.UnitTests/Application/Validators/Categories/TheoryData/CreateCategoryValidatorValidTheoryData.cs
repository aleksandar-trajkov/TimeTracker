using TimeTracker.Application.UseCases.Categories.Handlers;
using TimeTracker.UnitTests.Common.Builders.Application.Categories;

namespace TimeTracker.UnitTests.Application.Validators.Categories.TheoryData;

public class CreateCategoryValidatorValidTheoryData : TheoryData<CreateCategoryHandler.Command>
{
    public CreateCategoryValidatorValidTheoryData()
    {
        // Valid command with organization ID
        Add(new CreateCategoryCommandBuilder().WithName("Valid Category Name").WithDescription("Valid description").WithOrganizationId(Guid.NewGuid()).Build());

        // Valid command with null description
        Add(new CreateCategoryCommandBuilder().WithName("Valid Category Name").WithDescription(null).WithOrganizationId(Guid.NewGuid()).Build());

        // Valid command with empty organization ID (bypasses organization validation)
        Add(new CreateCategoryCommandBuilder().WithName("Valid Category Name").WithDescription("Valid description").WithOrganizationId(Guid.Empty).Build());

        // Valid command with null organization ID (bypasses organization validation)
        Add(new CreateCategoryCommandBuilder().WithName("Valid Category Name").WithDescription("Valid description").WithOrganizationId(null).Build());

        // Valid command with maximum length name (200 characters)
        Add(new CreateCategoryCommandBuilder().WithName(new string('a', 200)).WithDescription("Valid description").WithOrganizationId(Guid.NewGuid()).Build());

        // Valid command with maximum length description (1000 characters)
        Add(new CreateCategoryCommandBuilder().WithName("Valid name").WithDescription(new string('a', 1000)).WithOrganizationId(Guid.NewGuid()).Build());

        // Valid command with empty description
        Add(new CreateCategoryCommandBuilder().WithName("Valid name").WithDescription("").WithOrganizationId(Guid.NewGuid()).Build());

        // Valid command with whitespace only description (should pass as it's only validated when not null/empty)
        Add(new CreateCategoryCommandBuilder().WithName("Valid name").WithDescription("   ").WithOrganizationId(Guid.NewGuid()).Build());
    }
}
