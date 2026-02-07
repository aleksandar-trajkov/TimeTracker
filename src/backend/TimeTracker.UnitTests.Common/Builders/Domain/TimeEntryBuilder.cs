using TimeTracker.Domain;
using TimeTracker.UnitTests.Common.Extensions;

namespace TimeTracker.UnitTests.Common.Builders.Domain;

public class TimeEntryBuilder : EntityBuilder<TimeEntryBuilder, TimeEntry>
{
    private Guid _id = Guid.NewGuid();
    private DateTimeOffset _from = DateTimeOffset.Now.AddHours(-2);
    private DateTimeOffset _to = DateTimeOffset.Now;
    private string _description = Random.Shared.GenerateString(10, 100);
    private Guid _categoryId = Guid.NewGuid();
    private Guid _userId = Guid.NewGuid();

    protected override TimeEntry Instance => new TimeEntry
    {
        Id = _id,
        From = _from,
        To = _to,
        Description = _description,
        CategoryId = _categoryId,
        UserId = _userId
    };

    public TimeEntryBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public TimeEntryBuilder WithFrom(DateTimeOffset from)
    {
        _from = from;
        return this;
    }

    public TimeEntryBuilder WithTo(DateTimeOffset to)
    {
        _to = to;
        return this;
    }

    public TimeEntryBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public TimeEntryBuilder WithCategoryId(Guid categoryId)
    {
        _categoryId = categoryId;
        return this;
    }

    public TimeEntryBuilder WithUserId(Guid userId)
    {
        _userId = userId;
        return this;
    }

    public TimeEntryBuilder WithTimeRange(DateTimeOffset from, DateTimeOffset to)
    {
        _from = from;
        _to = to;
        return this;
    }
}