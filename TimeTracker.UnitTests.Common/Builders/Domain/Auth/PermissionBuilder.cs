using TimeTracker.Domain.Auth;
using TimeTracker.UnitTests.Common.Utilities;

namespace TimeTracker.UnitTests.Common.Builders.Domain.Auth;

public class PermissionBuilder : EntityBuilder<PermissionBuilder, Permission>
{
    private Guid _id = Guid.NewGuid();
    private string _name = RandomStringGenerator.GenerateString(20, 30);
    private string _description = RandomStringGenerator.GenerateString(200, 500);

    protected override Permission Instance => new Permission
    {
        Id = _id,
        Name = _name,
        Description = _description
    };

    public PermissionBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public PermissionBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public PermissionBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }
}