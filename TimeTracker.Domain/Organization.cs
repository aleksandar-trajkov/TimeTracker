using TimeTracker.Domain.Auth;
using TimeTracker.Domain.Base;

namespace TimeTracker.Domain;

public class Organization : BaseModel<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<Category> Categories { get; set; } = new List<Category>();
}