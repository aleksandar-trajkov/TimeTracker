using TimeTracker.Domain.Base;

namespace TimeTracker.Domain.Auth;

public class Permission : BaseModel<Guid>
{
    public PermissionEnum Key { get; init; }

    public Guid? EntityId { get; init; }

    public Guid UserId { get; init; }
    public User User { get; init; } = null!;
}