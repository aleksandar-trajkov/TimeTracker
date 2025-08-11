using FluentAssertions;
using TimeTracker.Application.UseCases.Categories.Handlers;
using TimeTracker.Application.UseCases.Categories.Validators;
using TimeTracker.UnitTests.Application.UseCases.Validators.Categories.TheoryData;
using TimeTracker.UnitTests.Common.Mocks.Data;

namespace TimeTracker.UnitTests.Application.UseCases.Validators.Categories;

public class UpdateCategoryValidatorTests
{
    private readonly CategoryRepositoryMockDouble _categoryRepository;
    private readonly UpdateCategoryValidator _validator;

    public UpdateCategoryValidatorTests()
    {
        _categoryRepository = new CategoryRepositoryMockDouble();
        _validator = new UpdateCategoryValidator(_categoryRepository.Instance);
    }

    [Fact]
    public async Task ValidCommand_ShouldPassValidation()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        _categoryRepository.GivenExistsAsync(categoryId, true);
        
        var command = new UpdateCategoryHandler.Command(
            categoryId,
            "Valid Category Name",
            "Valid description");

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task ValidCommandWithNullDescription_ShouldPassValidation()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        _categoryRepository.GivenExistsAsync(categoryId, true);
        
        var command = new UpdateCategoryHandler.Command(
            categoryId,
            "Valid Category Name",
            null);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [ClassData(typeof(IsUpdateCategoryCommandInvalidTheoryData))]
    public async Task InvalidCommand_ShouldFailValidation(UpdateCategoryHandler.Command command, string expectedErrorMessage)
    {
        // Arrange
        if (command.Id != Guid.Empty)
        {
            _categoryRepository.GivenExistsAsync(command.Id, false);
        }

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(error => error.ErrorMessage == expectedErrorMessage);
    }
}