using TimeTracker.Domain;
using TimeTracker.UnitTests.Common.Extensions;

namespace TimeTracker.UnitTests.Common.Builders.Domain;

public class OrganizationBuilder : EntityBuilder<OrganizationBuilder, Organization>
{
    private Guid _id = Guid.NewGuid();
    private string _name = Random.Shared.GenerateString(5, 20);
    private string? _description = Random.Shared.GenerateString(10, 50);

    protected override Organization Instance => new Organization
    {
        Id = _id,
        Name = _name,
        Description = _description
    };

    public OrganizationBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public OrganizationBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public OrganizationBuilder WithDescription(string? description)
    {
        _description = description;
        return this;
    }

    public OrganizationBuilder WithoutDescription()
    {
        _description = null;
        return this;
    }
}