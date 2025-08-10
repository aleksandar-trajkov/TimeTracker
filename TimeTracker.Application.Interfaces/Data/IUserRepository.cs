using TimeTracker.Application.Interfaces.Data.Base;
using TimeTracker.Domain.Auth;

namespace TimeTracker.Application.Interfaces.Data;

public interface IUserRepository : IBaseRepository<User, Guid>
{
    Task<IEnumerable<User>> GetAllAsync(Guid organizationId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}