using TimeTracker.Domain.Base;

namespace TimeTracker.Domain.Auth;

public class User : BaseModel<Guid>
{
    public string FirstName { get; init; } = string.Empty;
    public string? LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsActive { get; init; } = false;

    public IList<Permission> Permissions { get; private set; } = new List<Permission>();
}