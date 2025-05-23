using TimeTracker.Domain.Auth;
using TimeTracker.UnitTests.Common.Utilities;

namespace TimeTracker.UnitTests.Common.Builders.Domain.Auth;

public class PermissionBuilder : EntityBuilder<PermissionBuilder, Permission>
{
    private Guid _id = Guid.NewGuid();
    private PermissionEnum _key;

    protected override Permission Instance => new Permission
    {
        Id = _id,
        Key = _key
    };

    public PermissionBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public PermissionBuilder WithKey(PermissionEnum key)
    {
        _key = key;
        return this;
    }
}