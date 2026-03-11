using TimeTracker.Application.UseCases.TimeEntries.Handlers;
using TimeTracker.UnitTests.Common.Builders.Application.TimeEntries;

namespace TimeTracker.UnitTests.Application.Validators.TimeEntries.TheoryData;

public class GetTimeEntryByIdValidatorInvalidTheoryData : TheoryData<GetTimeEntryByIdHandler.Query, bool, bool, string>
{
    public GetTimeEntryByIdValidatorInvalidTheoryData()
    {
        Add(new GetTimeEntryByIdQueryBuilder()
                .WithId(Guid.Empty)
                .Build(),
            true,
            true,
            "Time entry ID is required.");

        Add(new GetTimeEntryByIdQueryBuilder()
                .WithId(Guid.NewGuid())
                .Build(),
            false,
            true,
            "Time entry does not exist.");

        Add(new GetTimeEntryByIdQueryBuilder()
                .WithId(Guid.NewGuid())
                .Build(),
            true,
            false,
            "You can only access your own time entries.");
    }
}
