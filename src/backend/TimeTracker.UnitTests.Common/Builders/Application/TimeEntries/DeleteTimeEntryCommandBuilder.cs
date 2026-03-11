using TimeTracker.Application.UseCases.TimeEntries.Handlers;

namespace TimeTracker.UnitTests.Common.Builders.Application.TimeEntries;

public class DeleteTimeEntryCommandBuilder : EntityBuilder<DeleteTimeEntryCommandBuilder, DeleteTimeEntryHandler.Command>
{
    private Guid _id = Guid.NewGuid();

    protected override DeleteTimeEntryHandler.Command Instance => new(_id);

    public DeleteTimeEntryCommandBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }
}
