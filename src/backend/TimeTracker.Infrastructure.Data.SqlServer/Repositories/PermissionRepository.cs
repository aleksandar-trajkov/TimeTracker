using TimeTracker.Application.Interfaces.Data;
using TimeTracker.Domain.Auth;
using TimeTracker.Infrastructure.Data.SqlServer.Base;
using TimeTracker.Infrastructure.Data.SqlServer.Interfaces;

namespace TimeTracker.Infrastructure.Data.SqlServer.Repositories;

public class PermissionRepository : BaseRepository<Permission, Guid>, IPermissionRepository
{
    public PermissionRepository(IDatabaseContext context) : base(context)
    {
    }
}