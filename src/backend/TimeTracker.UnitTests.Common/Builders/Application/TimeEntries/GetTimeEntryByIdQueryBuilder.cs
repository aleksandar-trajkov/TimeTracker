using TimeTracker.Application.UseCases.TimeEntries.Handlers;

namespace TimeTracker.UnitTests.Common.Builders.Application.TimeEntries;

public class GetTimeEntryByIdQueryBuilder : EntityBuilder<GetTimeEntryByIdQueryBuilder, GetTimeEntryByIdHandler.Query>
{
    private Guid _id = Guid.NewGuid();

    protected override GetTimeEntryByIdHandler.Query Instance => new(_id);

    public GetTimeEntryByIdQueryBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }
}
