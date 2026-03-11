using TimeTracker.Application.UseCases.TimeEntries.Handlers;
using TimeTracker.UnitTests.Common.Builders.Application.TimeEntries;

namespace TimeTracker.UnitTests.Application.Validators.TimeEntries.TheoryData;

public class DeleteTimeEntryValidatorInvalidTheoryData : TheoryData<DeleteTimeEntryHandler.Command, bool, bool, string>
{
    public DeleteTimeEntryValidatorInvalidTheoryData()
    {
        Add(new DeleteTimeEntryCommandBuilder()
                .WithId(Guid.Empty)
                .Build(),
            true,
            true,
            "Time entry ID is required.");

        Add(new DeleteTimeEntryCommandBuilder()
                .WithId(Guid.NewGuid())
                .Build(),
            false,
            true,
            "Time entry does not exist.");

        Add(new DeleteTimeEntryCommandBuilder()
                .WithId(Guid.NewGuid())
                .Build(),
            true,
            false,
            "You can only delete your own time entries.");
    }
}
