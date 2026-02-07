using Microsoft.EntityFrameworkCore;
using TimeTracker.Application.Interfaces.Data;
using TimeTracker.Domain;
using TimeTracker.Infrastructure.Data.SqlServer.Base;
using TimeTracker.Infrastructure.Data.SqlServer.Interfaces;

namespace TimeTracker.Infrastructure.Data.SqlServer.Repositories;

public class TimeEntryRepository : BaseRepository<TimeEntry, Guid>, ITimeEntryRepository
{
    public TimeEntryRepository(IDatabaseContext context) : base(context)
    {
    }

    public override async Task<TimeEntry> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await Select()
            .Include(x => x.Category)
            .SingleAsync(x => x.Id.Equals(id), cancellationToken);
    }

    public Task<List<TimeEntry>> GetAllAsync(Guid userId, DateTimeOffset from, DateTimeOffset to, CancellationToken cancellationToken)
    {
        return Select()
            .Include(te => te.Category)
            .Include(te => te.User)
            .Where(te => te.UserId == userId && te.From >= from && te.To <= to)
            .OrderBy(te => te.From)
            .ToListAsync(cancellationToken);
    }
}