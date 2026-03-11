using Bogus;
using TimeTracker.Application.UseCases.TimeEntries.Handlers;

namespace TimeTracker.UnitTests.Common.Builders.Application.TimeEntries;

public class CreateTimeEntryCommandBuilder : EntityBuilder<CreateTimeEntryCommandBuilder, CreateTimeEntryHandler.Command>
{
    private static readonly Faker Faker = new();

    private DateTimeOffset _from = DateTimeOffset.UtcNow.AddHours(-2);
    private DateTimeOffset _to = DateTimeOffset.UtcNow;
    private string _description = Faker.Lorem.Sentence();
    private Guid _categoryId = Guid.NewGuid();

    protected override CreateTimeEntryHandler.Command Instance => new(_from, _to, _description, _categoryId);

    public CreateTimeEntryCommandBuilder WithFrom(DateTimeOffset from)
    {
        _from = from;
        return this;
    }

    public CreateTimeEntryCommandBuilder WithTo(DateTimeOffset to)
    {
        _to = to;
        return this;
    }

    public CreateTimeEntryCommandBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public CreateTimeEntryCommandBuilder WithCategoryId(Guid categoryId)
    {
        _categoryId = categoryId;
        return this;
    }
}
