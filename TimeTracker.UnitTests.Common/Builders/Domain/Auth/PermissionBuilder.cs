using TimeTracker.Domain.Auth;

namespace TimeTracker.UnitTests.Common.Builders.Domain.Auth;

public class PermissionBuilder : EntityBuilder<PermissionBuilder, Permission>
{
    private Guid _id = Guid.NewGuid();
    private PermissionEnum _key;
    private Guid _userId = Guid.NewGuid();

    protected override Permission Instance => new Permission
    {
        Id = _id,
        Key = _key,
        UserId = _userId
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

    public PermissionBuilder WithUserId(Guid userId)
    {
        _userId = userId;
        return this;
    }
}