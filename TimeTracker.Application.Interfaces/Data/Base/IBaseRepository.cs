using System.Linq.Expressions;
using TimeTracker.Domain.Base;

namespace TimeTracker.Application.Interfaces.Data.Base;

public interface IBaseRepository<TEntity, TId> where TEntity : BaseModel<TId>
{
    Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken = default);
    Task<TEntity> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    Task<TEntity?> FindByIdAsync(TId id, CancellationToken cancellationToken = default);
    Task<int> InsertAsync(TEntity entity, bool persist = true, CancellationToken cancellationToken = default);
    Task<int> UpdateAsync(TEntity entity, bool persist = true, CancellationToken cancellationToken = default);
    Task<int> InsertOrUpdateAsync(Expression<Func<TEntity, bool>> comparer, TEntity entity, bool persist = true, CancellationToken cancellationToken = default);
    Task DeleteAsync(TId id, CancellationToken cancellationToken = default);
    Task<int> DeleteAsync(TEntity entity, bool persist = true, CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
