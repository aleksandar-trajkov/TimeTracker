using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TimeTracker.Application.Interfaces.Data.Base;
using TimeTracker.Domain.Base;
using TimeTracker.Infrastructure.Data.SqlServer.Interfaces;

namespace TimeTracker.Infrastructure.Data.SqlServer.Base;

public class BaseRepository<TEntity, TId> : IBaseRepository<TEntity, TId> where TEntity : BaseModel<TId> where TId : IEquatable<TId>
{
    public readonly IDatabaseContext _context;
    private DbSet<TEntity>? _entities;

    public BaseRepository(IDatabaseContext context)
    {
        _context = context;
    }

    protected virtual DbSet<TEntity> Entities
    {
        get
        {
            if (_entities == null)
                _entities = _context.Set<TEntity, TId>();
            return _entities;
        }
    }

    protected virtual IQueryable<TEntity> Select()
    {
        return Entities.AsNoTracking();
    }

    public Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken = default)
    {
        return Entities.AnyAsync(x => x.Id.Equals(id), cancellationToken);
    }

    public virtual async Task<TEntity> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        return await Entities.SingleAsync(x => x.Id.Equals(id), cancellationToken);
    }

    public async Task<TEntity?> FindByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        return await Entities.SingleOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);
    }

    public virtual async Task<int> InsertAsync(TEntity entity, bool persist = true, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        (_context as DbContext)!.Entry(entity).State = EntityState.Added;

        if (persist)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
        return 0;
    }

    public virtual async Task<int> UpdateAsync(TEntity entity, bool persist = true, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        (_context as DbContext)!.ChangeTracker.Clear();

        (_context as DbContext)!.Entry(entity).State = EntityState.Modified;

        if (persist)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
        return 0;
    }

    public virtual async Task<int> InsertOrUpdateAsync(Expression<Func<TEntity, bool>> comparer, TEntity entity, bool persist = true, CancellationToken cancellationToken = default)
    {
        if (!Entities.Any(comparer))
        {
            return await InsertAsync(entity, persist, cancellationToken);
        }
        else
        {
            return await UpdateAsync(entity, persist, cancellationToken);
        }
    }

    public virtual async Task<int> DeleteAsync(TEntity entity, bool persist = true, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        Entities.Remove(entity);


        if (persist)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
        return 0;
    }

    public virtual async Task<int> DeleteAsync(TId id, bool persist = true, CancellationToken cancellationToken = default)
    {
        return await Entities.Where(x => x.Id.Equals(id)).ExecuteDeleteAsync(cancellationToken);

        if (persist)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
        return 0;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
