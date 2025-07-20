using TimeTracker.Application.Interfaces.Data;
using TimeTracker.Domain;
using TimeTracker.Infrastructure.Data.SqlServer.Base;
using TimeTracker.Infrastructure.Data.SqlServer.Interfaces;

namespace TimeTracker.Infrastructure.Data.SqlServer;

public class CategoryRepository : BaseRepository<Category, Guid>, ICategoryRepository
{
    public CategoryRepository(IDatabaseContext context) : base(context)
    {
    }
}