using AwesomeAssertions;
using TimeTracker.Application.UseCases.TimeEntries.Validators;
using TimeTracker.UnitTests.Application.Validators.TimeEntries.TheoryData;
using TimeTracker.UnitTests.Common.Builders.Application.TimeEntries;
using TimeTracker.UnitTests.Common.Builders.Domain;
using TimeTracker.UnitTests.Common.Mocks.Auth;
using TimeTracker.UnitTests.Common.Mocks.Data;

namespace TimeTracker.UnitTests.Application.Validators.TimeEntries;

public class UpdateTimeEntryValidatorTests
{
    private readonly TimeEntryRepositoryMockDouble _timeEntryRepository;
    private readonly CategoryRepositoryMockDouble _categoryRepository;
    private readonly UserContextMockDouble _userContext;
    private readonly UpdateTimeEntryValidator _validator;

    public UpdateTimeEntryValidatorTests()
    {
        _timeEntryRepository = new TimeEntryRepositoryMockDouble();
        _categoryRepository = new CategoryRepositoryMockDouble();
        _userContext = new UserContextMockDouble();
        _validator = new UpdateTimeEntryValidator(_timeEntryRepository.Instance, _categoryRepository.Instance, _userContext.Instance);
    }

    [Fact]
    public async Task ValidCommand_ShouldPassValidation()
    {
        var userId = Guid.NewGuid();
        var timeEntryId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();

        _userContext.GivenUserId(userId);
        _timeEntryRepository.GivenExistsAsync(timeEntryId, true);
        _timeEntryRepository.GivenFindByIdAsync(timeEntryId, new TimeEntryBuilder().WithId(timeEntryId).WithUserId(userId).Build());
        _categoryRepository.GivenExistsAsync(categoryId, true);

        var command = new UpdateTimeEntryCommandBuilder()
            .WithId(timeEntryId)
            .WithCategoryId(categoryId)
            .WithDescription("Valid description")
            .WithFrom(DateTimeOffset.UtcNow.AddHours(-2))
            .WithTo(DateTimeOffset.UtcNow)
            .Build();

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [ClassData(typeof(UpdateTimeEntryValidatorInvalidTheoryData))]
    public async Task InvalidCommand_ShouldFailValidation(
        TimeTracker.Application.UseCases.TimeEntries.Handlers.UpdateTimeEntryHandler.Command command,
        bool timeEntryExists,
        bool userOwnsTimeEntry,
        bool categoryExists,
        string expectedErrorMessage)
    {
        var userId = Guid.NewGuid();
        _userContext.GivenUserId(userId);

        if (command.Id != Guid.Empty)
        {
            _timeEntryRepository.GivenExistsAsync(command.Id, timeEntryExists);
            var ownerId = userOwnsTimeEntry ? userId : Guid.NewGuid();
            _timeEntryRepository.GivenFindByIdAsync(command.Id, timeEntryExists ? new TimeEntryBuilder().WithId(command.Id).WithUserId(ownerId).Build() : null);
        }

        if (command.CategoryId != Guid.Empty)
        {
            _categoryRepository.GivenExistsAsync(command.CategoryId, categoryExists);
        }

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.ErrorMessage == expectedErrorMessage);
    }
}
