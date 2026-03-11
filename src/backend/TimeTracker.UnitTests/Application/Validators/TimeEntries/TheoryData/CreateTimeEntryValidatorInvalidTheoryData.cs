using TimeTracker.Application.UseCases.TimeEntries.Handlers;
using TimeTracker.UnitTests.Common.Builders.Application.TimeEntries;

namespace TimeTracker.UnitTests.Application.Validators.TimeEntries.TheoryData;

public class CreateTimeEntryValidatorInvalidTheoryData : TheoryData<CreateTimeEntryHandler.Command, bool, string>
{
    public CreateTimeEntryValidatorInvalidTheoryData()
    {
        Add(new CreateTimeEntryCommandBuilder()
                .WithFrom(default)
                .WithTo(DateTimeOffset.UtcNow)
                .WithDescription("Valid description")
                .WithCategoryId(Guid.NewGuid())
                .Build(),
            true,
            "Start time is required.");

        Add(new CreateTimeEntryCommandBuilder()
                .WithFrom(DateTimeOffset.UtcNow)
                .WithTo(DateTimeOffset.UtcNow.AddHours(-1))
                .WithDescription("Valid description")
                .WithCategoryId(Guid.NewGuid())
                .Build(),
            true,
            "Start time must be earlier than end time.");

        Add(new CreateTimeEntryCommandBuilder()
                .WithFrom(DateTimeOffset.UtcNow.AddHours(-1))
                .WithTo(default)
                .WithDescription("Valid description")
                .WithCategoryId(Guid.NewGuid())
                .Build(),
            true,
            "End time is required.");

        Add(new CreateTimeEntryCommandBuilder()
                .WithDescription(string.Empty)
                .Build(),
            true,
            "Description is required.");

        Add(new CreateTimeEntryCommandBuilder()
                .WithDescription(new string('a', 1001))
                .Build(),
            true,
            "Description cannot exceed 1000 characters.");

        Add(new CreateTimeEntryCommandBuilder()
                .WithCategoryId(Guid.Empty)
                .Build(),
            true,
            "Category is required.");

        Add(new CreateTimeEntryCommandBuilder()
                .WithCategoryId(Guid.NewGuid())
                .Build(),
            false,
            "Category does not exist.");

        Add(new CreateTimeEntryCommandBuilder()
                .WithFrom(DateTimeOffset.UtcNow.AddDays(-2))
                .WithTo(DateTimeOffset.UtcNow)
                .WithDescription("Valid description")
                .WithCategoryId(Guid.NewGuid())
                .Build(),
            true,
            "Time entry cannot exceed 24 hours.");
    }
}
