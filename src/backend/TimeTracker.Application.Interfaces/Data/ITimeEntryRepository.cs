using TimeTracker.Application.Interfaces.Data.Base;
using TimeTracker.Domain;

namespace TimeTracker.Application.Interfaces.Data;

public interface ITimeEntryRepository : IBaseRepository<TimeEntry, Guid>
{
    Task<List<TimeEntry>> GetAllAsync(Guid userId, DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken);
}