using AwesomeAssertions;
using TimeTracker.Application.UseCases.TimeEntries.Validators;
using TimeTracker.UnitTests.Application.Validators.TimeEntries.TheoryData;
using TimeTracker.UnitTests.Common.Builders.Application.TimeEntries;
using TimeTracker.UnitTests.Common.Builders.Domain;
using TimeTracker.UnitTests.Common.Mocks.Auth;
using TimeTracker.UnitTests.Common.Mocks.Data;

namespace TimeTracker.UnitTests.Application.Validators.TimeEntries;

public class GetTimeEntryByIdValidatorTests
{
    private readonly TimeEntryRepositoryMockDouble _timeEntryRepository;
    private readonly UserContextMockDouble _userContext;
    private readonly GetTimeEntryByIdValidator _validator;

    public GetTimeEntryByIdValidatorTests()
    {
        _timeEntryRepository = new TimeEntryRepositoryMockDouble();
        _userContext = new UserContextMockDouble();
        _validator = new GetTimeEntryByIdValidator(_timeEntryRepository.Instance, _userContext.Instance);
    }

    [Fact]
    public async Task ValidQuery_ShouldPassValidation()
    {
        var userId = Guid.NewGuid();
        var timeEntryId = Guid.NewGuid();

        _userContext.GivenUserId(userId);
        _timeEntryRepository.GivenExistsAsync(timeEntryId, true);
        _timeEntryRepository.GivenFindByIdAsync(timeEntryId, new TimeEntryBuilder().WithId(timeEntryId).WithUserId(userId).Build());

        var query = new GetTimeEntryByIdQueryBuilder()
            .WithId(timeEntryId)
            .Build();

        var result = await _validator.ValidateAsync(query);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [ClassData(typeof(GetTimeEntryByIdValidatorInvalidTheoryData))]
    public async Task InvalidQuery_ShouldFailValidation(
        TimeTracker.Application.UseCases.TimeEntries.Handlers.GetTimeEntryByIdHandler.Query query,
        bool timeEntryExists,
        bool userOwnsTimeEntry,
        string expectedErrorMessage)
    {
        var userId = Guid.NewGuid();
        _userContext.GivenUserId(userId);

        if (query.Id != Guid.Empty)
        {
            _timeEntryRepository.GivenExistsAsync(query.Id, timeEntryExists);
            var ownerId = userOwnsTimeEntry ? userId : Guid.NewGuid();
            _timeEntryRepository.GivenFindByIdAsync(query.Id, timeEntryExists ? new TimeEntryBuilder().WithId(query.Id).WithUserId(ownerId).Build() : null);
        }

        var result = await _validator.ValidateAsync(query);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.ErrorMessage == expectedErrorMessage);
    }
}
