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

    [Theory]
    [ClassData(typeof(ValidUpdateCategoryCommandTheoryData))]
    public async Task ValidCommand_ShouldPassValidation(UpdateCategoryHandler.Command command)
    {
        // Arrange
        _categoryRepository.GivenExistsAsync(command.Id, true);

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