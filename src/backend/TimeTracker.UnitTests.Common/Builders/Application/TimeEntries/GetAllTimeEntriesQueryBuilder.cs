using TimeTracker.Application.UseCases.TimeEntries.Handlers;

namespace TimeTracker.UnitTests.Common.Builders.Application.TimeEntries;

public class GetAllTimeEntriesQueryBuilder : EntityBuilder<GetAllTimeEntriesQueryBuilder, GetAllTimeEntriesHandler.Query>
{
    private DateTimeOffset _from = DateTimeOffset.UtcNow.AddDays(-7);
    private DateTimeOffset _to = DateTimeOffset.UtcNow;

    protected override GetAllTimeEntriesHandler.Query Instance => new(_from, _to);

    public GetAllTimeEntriesQueryBuilder WithFrom(DateTimeOffset from)
    {
        _from = from;
        return this;
    }

    public GetAllTimeEntriesQueryBuilder WithTo(DateTimeOffset to)
    {
        _to = to;
        return this;
    }
}
