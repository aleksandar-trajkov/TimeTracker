using Microsoft.EntityFrameworkCore;
using TimeTracker.Application.Interfaces.Data;
using TimeTracker.Domain.Auth;
using TimeTracker.Infrastructure.Data.SqlServer.Base;
using TimeTracker.Infrastructure.Data.SqlServer.Interfaces;

namespace TimeTracker.Infrastructure.Data.SqlServer.Repositories;

public class UserRepository : BaseRepository<User, Guid>, IUserRepository
{
    public UserRepository(IDatabaseContext context) : base(context)
    {
    }

    public async Task<IEnumerable<User>> GetAllAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        return await this.Select()
            .Include(u => u.Permissions)
            .Where(u => u.OrganizationId == organizationId)
            .ToListAsync(cancellationToken);
    }


    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return this.Select()
            .AnyAsync(u => u.Email == email, cancellationToken);
    }
    public async Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await this.Select()
            .Include(x => x.Permissions)
            .SingleAsync(u => u.Email == email, cancellationToken);
    }
}
