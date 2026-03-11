using TimeTracker.Application.UseCases.TimeEntries.Handlers;
using TimeTracker.UnitTests.Common.Builders.Application.TimeEntries;

namespace TimeTracker.UnitTests.Application.Validators.TimeEntries.TheoryData;

public class GetAllTimeEntriesValidatorInvalidTheoryData : TheoryData<GetAllTimeEntriesHandler.Query, string>
{
    public GetAllTimeEntriesValidatorInvalidTheoryData()
    {
        Add(new GetAllTimeEntriesQueryBuilder()
                .WithFrom(default)
                .WithTo(DateTimeOffset.UtcNow)
                .Build(),
            "Start date is required.");

        Add(new GetAllTimeEntriesQueryBuilder()
                .WithFrom(DateTimeOffset.UtcNow.AddDays(-1))
                .WithTo(default)
                .Build(),
            "End date is required.");

        Add(new GetAllTimeEntriesQueryBuilder()
                .WithFrom(DateTimeOffset.UtcNow)
                .WithTo(DateTimeOffset.UtcNow.AddDays(-1))
                .Build(),
            "Start date must be earlier than or equal to end date.");

        Add(new GetAllTimeEntriesQueryBuilder()
                .WithFrom(DateTimeOffset.UtcNow.AddDays(-366))
                .WithTo(DateTimeOffset.UtcNow)
                .Build(),
            "Date range cannot exceed 365 days.");
    }
}
