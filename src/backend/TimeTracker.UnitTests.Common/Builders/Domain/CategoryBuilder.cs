using TimeTracker.Domain;
using Bogus;

namespace TimeTracker.UnitTests.Common.Builders.Domain;

public class CategoryBuilder : EntityBuilder<CategoryBuilder, Category>
{
    private static readonly Faker Faker = new();

    private Guid _id = Guid.NewGuid();
    private string _name = Faker.Commerce.Categories(1).First();
    private string? _description = Faker.Lorem.Sentence();
    private Guid _organizationId = Guid.NewGuid();
    private IEnumerable<TimeEntry> _timeEntries = new List<TimeEntry>();

    protected override Category Instance => new Category
    {
        Id = _id,
        Name = _name,
        Description = _description,
        OrganizationId = _organizationId,
        TimeEntries = _timeEntries.ToList(),
    };

    public CategoryBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public CategoryBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public CategoryBuilder WithDescription(string? description)
    {
        _description = description;
        return this;
    }

    public CategoryBuilder WithOrganizationId(Guid organizationId)
    {
        _organizationId = organizationId;
        return this;
    }

    public CategoryBuilder WithoutDescription()
    {
        _description = null;
        return this;
    }

    public CategoryBuilder WithTimeEntries(params IEnumerable<TimeEntry> timeEntries)
    {
        _timeEntries = timeEntries;
        return this;
    }
}