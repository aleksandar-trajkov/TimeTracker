using AwesomeAssertions;
using TimeTracker.Application.UseCases.TimeEntries.Validators;
using TimeTracker.UnitTests.Application.Validators.TimeEntries.TheoryData;
using TimeTracker.UnitTests.Common.Builders.Application.TimeEntries;
using TimeTracker.UnitTests.Common.Builders.Domain;
using TimeTracker.UnitTests.Common.Mocks.Auth;
using TimeTracker.UnitTests.Common.Mocks.Data;

namespace TimeTracker.UnitTests.Application.Validators.TimeEntries;

public class DeleteTimeEntryValidatorTests
{
    private readonly TimeEntryRepositoryMockDouble _timeEntryRepository;
    private readonly UserContextMockDouble _userContext;
    private readonly DeleteTimeEntryValidator _validator;

    public DeleteTimeEntryValidatorTests()
    {
        _timeEntryRepository = new TimeEntryRepositoryMockDouble();
        _userContext = new UserContextMockDouble();
        _validator = new DeleteTimeEntryValidator(_timeEntryRepository.Instance, _userContext.Instance);
    }

    [Fact]
    public async Task ValidCommand_ShouldPassValidation()
    {
        var userId = Guid.NewGuid();
        var timeEntryId = Guid.NewGuid();

        _userContext.GivenUserId(userId);
        _timeEntryRepository.GivenExistsAsync(timeEntryId, true);
        _timeEntryRepository.GivenFindByIdAsync(timeEntryId, new TimeEntryBuilder().WithId(timeEntryId).WithUserId(userId).Build());

        var command = new DeleteTimeEntryCommandBuilder()
            .WithId(timeEntryId)
            .Build();

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [ClassData(typeof(DeleteTimeEntryValidatorInvalidTheoryData))]
    public async Task InvalidCommand_ShouldFailValidation(
        TimeTracker.Application.UseCases.TimeEntries.Handlers.DeleteTimeEntryHandler.Command command,
        bool timeEntryExists,
        bool userOwnsTimeEntry,
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

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.ErrorMessage == expectedErrorMessage);
    }
}
