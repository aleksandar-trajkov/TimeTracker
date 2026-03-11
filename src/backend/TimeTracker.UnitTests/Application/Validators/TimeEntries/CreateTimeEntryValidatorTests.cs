using AwesomeAssertions;
using TimeTracker.Application.UseCases.TimeEntries.Validators;
using TimeTracker.UnitTests.Application.Validators.TimeEntries.TheoryData;
using TimeTracker.UnitTests.Common.Builders.Application.TimeEntries;
using TimeTracker.UnitTests.Common.Mocks.Data;

namespace TimeTracker.UnitTests.Application.Validators.TimeEntries;

public class CreateTimeEntryValidatorTests
{
    private readonly CategoryRepositoryMockDouble _categoryRepository;
    private readonly CreateTimeEntryValidator _validator;

    public CreateTimeEntryValidatorTests()
    {
        _categoryRepository = new CategoryRepositoryMockDouble();
        _validator = new CreateTimeEntryValidator(_categoryRepository.Instance);
    }

    [Fact]
    public async Task ValidCommand_ShouldPassValidation()
    {
        var categoryId = Guid.NewGuid();
        _categoryRepository.GivenExistsAsync(categoryId, true);

        var command = new CreateTimeEntryCommandBuilder()
            .WithCategoryId(categoryId)
            .WithFrom(DateTimeOffset.UtcNow.AddHours(-2))
            .WithTo(DateTimeOffset.UtcNow)
            .WithDescription("Valid description")
            .Build();

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [ClassData(typeof(CreateTimeEntryValidatorInvalidTheoryData))]
    public async Task InvalidCommand_ShouldFailValidation(
        TimeTracker.Application.UseCases.TimeEntries.Handlers.CreateTimeEntryHandler.Command command,
        bool categoryExists,
        string expectedErrorMessage)
    {
        if (command.CategoryId != Guid.Empty)
        {
            _categoryRepository.GivenExistsAsync(command.CategoryId, categoryExists);
        }

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.ErrorMessage == expectedErrorMessage);
    }
}
