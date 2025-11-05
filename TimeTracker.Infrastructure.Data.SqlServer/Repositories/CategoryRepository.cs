using Microsoft.EntityFrameworkCore;
using TimeTracker.Application.Interfaces.Data;
using TimeTracker.Domain;
using TimeTracker.Infrastructure.Data.SqlServer.Base;
using TimeTracker.Infrastructure.Data.SqlServer.Interfaces;

namespace TimeTracker.Infrastructure.Data.SqlServer.Repositories;

public class CategoryRepository : BaseRepository<Category, Guid>, ICategoryRepository
{
    public CategoryRepository(IDatabaseContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Category>> GetAllAsync(Guid organizationId, CancellationToken cancellationToken)
    {
        return await Select()
            .Where(c => c.OrganizationId == organizationId || c.OrganizationId == null)
            .ToListAsync(cancellationToken);
    }
}