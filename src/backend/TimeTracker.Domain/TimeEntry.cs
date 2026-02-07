using TimeTracker.Domain.Auth;
using TimeTracker.Domain.Base;

namespace TimeTracker.Domain;

public class TimeEntry : BaseModel<Guid>
{
    public DateTimeOffset From { get; set; }
    public DateTimeOffset To { get; set; }
    public string Description { get; set; } = string.Empty;

    public Guid CategoryId { get; set; }
    public Guid UserId { get; set; }
    public Category Category { get; set; } = null!;
    public User User { get; set; } = null!;
}