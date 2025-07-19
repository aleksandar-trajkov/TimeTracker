using TimeTracker.Domain.Base;

namespace TimeTracker.Domain.Auth;

public class UserVerificationCode : BaseModel<Guid>
{

    public string Code { get; set; } = string.Empty;

    public bool IsUsed { get; set; } = false;

    public DateTime ExpiresAt { get; set; }

    public Guid UserId { get; set; }

    public User User { get; set; } = null!;
}