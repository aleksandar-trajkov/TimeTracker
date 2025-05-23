using TimeTracker.Domain.Base;

namespace TimeTracker.Domain.Auth;

public class Permission : BaseModel<Guid>
{
    public PermissionEnum Key { get; init; }

    public User User { get; init; } = default!;
}