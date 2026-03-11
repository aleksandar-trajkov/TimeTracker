using TimeTracker.Application.UseCases.Categories.Handlers;
using TimeTracker.UnitTests.Common.Builders.Application.Categories;

namespace TimeTracker.UnitTests.Application.Validators.Categories.TheoryData;

public class CreateCategoryValidatorInvalidTheoryData : TheoryData<CreateCategoryHandler.Command, string>
{
    public CreateCategoryValidatorInvalidTheoryData()
    {
        // Empty name
        Add(new CreateCategoryCommandBuilder().WithName(string.Empty).WithDescription("Valid description").WithOrganizationId(Guid.NewGuid()).Build(),
            "Category name is required.");

        // Null name
        Add(new CreateCategoryCommandBuilder().WithName(null!).WithDescription("Valid description").WithOrganizationId(Guid.NewGuid()).Build(),
            "Category name is required.");

        // Whitespace only name
        Add(new CreateCategoryCommandBuilder().WithName("   ").WithDescription("Valid description").WithOrganizationId(Guid.NewGuid()).Build(),
            "Category name is required.");

        // Name too long (201 characters)
        Add(new CreateCategoryCommandBuilder().WithName(new string('a', 201)).WithDescription("Valid description").WithOrganizationId(Guid.NewGuid()).Build(),
            "Category name cannot exceed 200 characters.");

        // Description too long (1001 characters)
        Add(new CreateCategoryCommandBuilder().WithName("Valid name").WithDescription(new string('a', 1001)).WithOrganizationId(Guid.NewGuid()).Build(),
            "Category description cannot exceed 1000 characters.");

        // Organization does not exist
        Add(new CreateCategoryCommandBuilder().WithName("Valid name").WithDescription("Valid description").WithOrganizationId(Guid.NewGuid()).Build(),
            "Organization does not exist.");
    }
}
