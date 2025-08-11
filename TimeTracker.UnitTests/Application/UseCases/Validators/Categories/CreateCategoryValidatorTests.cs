using FluentAssertions;
using TimeTracker.Application.UseCases.Categories.Handlers;
using TimeTracker.Application.UseCases.Categories.Validators;
using TimeTracker.UnitTests.Application.UseCases.Validators.Categories.TheoryData;
using TimeTracker.UnitTests.Common.Mocks.Data;

namespace TimeTracker.UnitTests.Application.UseCases.Validators.Categories;

public class CreateCategoryValidatorTests
{
    private readonly OrganizationRepositoryMockDouble _organizationRepository;
    private readonly CreateCategoryValidator _validator;

    public CreateCategoryValidatorTests()
    {
        _organizationRepository = new OrganizationRepositoryMockDouble();
        _validator = new CreateCategoryValidator(_organizationRepository.Instance);
    }

    [Fact]
    public async Task ValidCommand_ShouldPassValidation()
    {
        // Arrange
        var organizationId = Guid.NewGuid();
        _organizationRepository.GivenExistsAsync(organizationId, true);
        
        var command = new CreateCategoryHandler.Command(
            "Valid Category Name",
            "Valid description",
            organizationId);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task ValidCommandWithNullDescription_ShouldPassValidation()
    {
        // Arrange
        var organizationId = Guid.NewGuid();
        _organizationRepository.GivenExistsAsync(organizationId, true);
        
        var command = new CreateCategoryHandler.Command(
            "Valid Category Name",
            null,
            organizationId);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task ValidCommandWithEmptyOrganizationId_ShouldPassValidation()
    {
        // Arrange
        var command = new CreateCategoryHandler.Command(
            "Valid Category Name",
            "Valid description",
            Guid.Empty);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task ValidCommandWithNullOrganizationId_ShouldPassValidation()
    {
        // Arrange
        var command = new CreateCategoryHandler.Command(
            "Valid Category Name",
            "Valid description",
            null);

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