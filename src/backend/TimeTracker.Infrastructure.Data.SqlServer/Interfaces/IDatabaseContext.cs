using Microsoft.EntityFrameworkCore;
using TimeTracker.Domain.Base;

namespace TimeTracker.Infrastructure.Data.SqlServer.Interfaces;

public interface IDatabaseContext
{
    DbSet<TEntity> Set<TEntity, TId>() where TEntity : BaseModel<TId>;
    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
