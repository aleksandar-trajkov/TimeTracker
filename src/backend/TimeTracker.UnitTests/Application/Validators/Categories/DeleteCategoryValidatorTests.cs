using AwesomeAssertions;
using TimeTracker.Application.UseCases.Categories.Handlers;
using TimeTracker.Application.UseCases.Categories.Validators;
using TimeTracker.UnitTests.Application.Validators.Categories.TheoryData;
using TimeTracker.UnitTests.Common.Builders.Application.Categories;
using TimeTracker.UnitTests.Common.Mocks.Data;

namespace TimeTracker.UnitTests.Application.Validators.Categories;

public class DeleteCategoryValidatorTests
{
    private readonly CategoryRepositoryMockDouble _categoryRepository;
    private readonly DeleteCategoryValidator _validator;

    public DeleteCategoryValidatorTests()
    {
        _categoryRepository = new CategoryRepositoryMockDouble();
        _validator = new DeleteCategoryValidator(_categoryRepository.Instance);
    }

    [Fact]
    public async Task ValidCommand_ShouldPassValidation()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        _categoryRepository.GivenExistsAsync(categoryId, true);

        var command = new DeleteCategoryCommandBuilder()
            .WithId(categoryId)
            .Build();

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [ClassData(typeof(DeleteCategoryValidatorInvalidTheoryData))]
    public async Task InvalidCommand_ShouldFailValidation(DeleteCategoryHandler.Command command, string expectedErrorMessage)
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
