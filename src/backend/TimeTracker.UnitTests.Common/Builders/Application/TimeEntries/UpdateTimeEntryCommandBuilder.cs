using Bogus;
using TimeTracker.Application.UseCases.TimeEntries.Handlers;

namespace TimeTracker.UnitTests.Common.Builders.Application.TimeEntries;

public class UpdateTimeEntryCommandBuilder : EntityBuilder<UpdateTimeEntryCommandBuilder, UpdateTimeEntryHandler.Command>
{
    private static readonly Faker Faker = new();

    private Guid _id = Guid.NewGuid();
    private DateTimeOffset _from = DateTimeOffset.UtcNow.AddHours(-2);
    private DateTimeOffset _to = DateTimeOffset.UtcNow;
    private string _description = Faker.Lorem.Sentence();
    private Guid _categoryId = Guid.NewGuid();

    protected override UpdateTimeEntryHandler.Command Instance => new(_id, _from, _to, _description, _categoryId);

    public UpdateTimeEntryCommandBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public UpdateTimeEntryCommandBuilder WithFrom(DateTimeOffset from)
    {
        _from = from;
        return this;
    }

    public UpdateTimeEntryCommandBuilder WithTo(DateTimeOffset to)
    {
        _to = to;
        return this;
    }

    public UpdateTimeEntryCommandBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public UpdateTimeEntryCommandBuilder WithCategoryId(Guid categoryId)
    {
        _categoryId = categoryId;
        return this;
    }
}
