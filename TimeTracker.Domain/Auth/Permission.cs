using TimeTracker.Domain.Base;

namespace TimeTracker.Domain.Auth;

public class Permission : BaseModel<Guid>
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}