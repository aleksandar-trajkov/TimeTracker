using AwesomeAssertions;
using TimeTracker.Application.UseCases.TimeEntries.Validators;
using TimeTracker.UnitTests.Application.Validators.TimeEntries.TheoryData;

namespace TimeTracker.UnitTests.Application.Validators.TimeEntries;

public class GetAllTimeEntriesValidatorTests
{
    private readonly GetAllTimeEntriesValidator _validator;

    public GetAllTimeEntriesValidatorTests()
    {
        _validator = new GetAllTimeEntriesValidator();
    }

    [Fact]
    public async Task ValidQuery_ShouldPassValidation()
    {
        var query = new TimeTracker.UnitTests.Common.Builders.Application.TimeEntries.GetAllTimeEntriesQueryBuilder()
            .WithFrom(DateTimeOffset.UtcNow.AddDays(-30))
            .WithTo(DateTimeOffset.UtcNow)
            .Build();

        var result = await _validator.ValidateAsync(query);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [ClassData(typeof(GetAllTimeEntriesValidatorInvalidTheoryData))]
    public async Task InvalidQuery_ShouldFailValidation(
        TimeTracker.Application.UseCases.TimeEntries.Handlers.GetAllTimeEntriesHandler.Query query,
        string expectedErrorMessage)
    {
        var result = await _validator.ValidateAsync(query);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(error => error.ErrorMessage == expectedErrorMessage);
    }
}
