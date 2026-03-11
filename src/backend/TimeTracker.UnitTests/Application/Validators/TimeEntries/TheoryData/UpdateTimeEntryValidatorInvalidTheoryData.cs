using TimeTracker.Application.UseCases.TimeEntries.Handlers;
using TimeTracker.UnitTests.Common.Builders.Application.TimeEntries;

namespace TimeTracker.UnitTests.Application.Validators.TimeEntries.TheoryData;

public class UpdateTimeEntryValidatorInvalidTheoryData : TheoryData<UpdateTimeEntryHandler.Command, bool, bool, bool, string>
{
    public UpdateTimeEntryValidatorInvalidTheoryData()
    {
        Add(new UpdateTimeEntryCommandBuilder()
                .WithId(Guid.Empty)
                .Build(),
            true,
            true,
            true,
            "Time entry ID is required.");

        Add(new UpdateTimeEntryCommandBuilder()
                .WithId(Guid.NewGuid())
                .Build(),
            false,
            true,
            true,
            "Time entry does not exist.");

        Add(new UpdateTimeEntryCommandBuilder()
                .WithId(Guid.NewGuid())
                .Build(),
            true,
            false,
            true,
            "You can only update your own time entries.");

        Add(new UpdateTimeEntryCommandBuilder()
                .WithFrom(DateTimeOffset.UtcNow)
                .WithTo(DateTimeOffset.UtcNow.AddHours(-1))
                .Build(),
            true,
            true,
            true,
            "Start time must be earlier than end time.");

        Add(new UpdateTimeEntryCommandBuilder()
                .WithTo(default)
                .Build(),
            true,
            true,
            true,
            "End time is required.");

        Add(new UpdateTimeEntryCommandBuilder()
                .WithDescription(new string('a', 1001))
                .Build(),
            true,
            true,
            true,
            "Description cannot exceed 1000 characters.");

        Add(new UpdateTimeEntryCommandBuilder()
                .WithCategoryId(Guid.Empty)
                .Build(),
            true,
            true,
            true,
            "Category is required.");

        Add(new UpdateTimeEntryCommandBuilder()
                .WithCategoryId(Guid.NewGuid())
                .Build(),
            true,
            true,
            false,
            "Category does not exist.");

        Add(new UpdateTimeEntryCommandBuilder()
                .WithFrom(DateTimeOffset.UtcNow.AddDays(-2))
                .WithTo(DateTimeOffset.UtcNow)
                .Build(),
            true,
            true,
            true,
            "Time entry cannot exceed 24 hours.");
    }
}
