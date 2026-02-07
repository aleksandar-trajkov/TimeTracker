using TimeTracker.Application.Interfaces.Data;
using TimeTracker.Domain;
using TimeTracker.Infrastructure.Data.SqlServer.Base;
using TimeTracker.Infrastructure.Data.SqlServer.Interfaces;

namespace TimeTracker.Infrastructure.Data.SqlServer.Repositories;

public class OrganizationRepository : BaseRepository<Organization, Guid>, IOrganizationRepository
{
    public OrganizationRepository(IDatabaseContext context) : base(context)
    {
    }
}