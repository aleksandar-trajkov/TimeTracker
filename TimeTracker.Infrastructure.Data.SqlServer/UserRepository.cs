using Microsoft.EntityFrameworkCore;
using TimeTracker.Application.Interfaces.Data;
using TimeTracker.Domain.Auth;
using TimeTracker.Infrastructure.Data.SqlServer.Base;
using TimeTracker.Infrastructure.Data.SqlServer.Interfaces;

namespace TimeTracker.Infrastructure.Data.SqlServer;

public class UserRepository : BaseRepository<User, Guid>, IUserRepository
{
    public UserRepository(IDatabaseContext context) : base(context)
    {
    }

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return this.Entities
            .AnyAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await this.Entities
            .Include(x => x.Permissions)
            .SingleAsync(u => u.Email == email, cancellationToken);
    }
}
