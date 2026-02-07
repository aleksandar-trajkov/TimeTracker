using TimeTracker.Domain.Base;

namespace TimeTracker.Domain;

public class Category : BaseModel<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public Guid? OrganizationId { get; set; }
    public Organization? Organization { get; set; }

    public ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
}