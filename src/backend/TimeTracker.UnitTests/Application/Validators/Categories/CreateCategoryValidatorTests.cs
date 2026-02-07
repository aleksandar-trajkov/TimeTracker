using FluentAssertions;
using TimeTracker.Application.UseCases.Categories.Handlers;
using TimeTracker.Application.UseCases.Categories.Validators;
using TimeTracker.UnitTests.Application.Validators.Categories.TheoryData;
using TimeTracker.UnitTests.Common.Mocks.Data;

namespace TimeTracker.UnitTests.Application.Validators.Categories;

public class CreateCategoryValidatorTests
{
    private readonly OrganizationRepositoryMockDouble _organizationRepository;
    private readonly CreateCategoryValidator _validator;

    public CreateCategoryValidatorTests()
    {
        _organizationRepository = new OrganizationRepositoryMockDouble();
        _validator = new CreateCategoryValidator(_organizationRepository.Instance);
    }

    [Theory]
    [ClassData(typeof(ValidCreateCategoryCommandTheoryData))]
    public async Task ValidCommand_ShouldPassValidation(CreateCategoryHandler.Command command)
    {
        // Arrange
        if (command.OrganizationId.HasValue && command.OrganizationId != Guid.Empty)
        {
            _organizationRepository.GivenExistsAsync(command.OrganizationId.Value, true);
        }

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [ClassData(typeof(IsCreateCategoryCommandInvalidTheoryData))]
    public async Task InvalidCommand_ShouldFailValidation(CreateCategoryHandler.Command command, string expectedErrorMessage)
    {
        // Arrange
        if (command.OrganizationId.HasValue && command.OrganizationId != Guid.Empty)
        {
            _organizationRepository.GivenExistsAsync(command.OrganizationId.Value, false);
        }

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.ErrorMessage == expectedErrorMessage);
    }
}