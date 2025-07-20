using Microsoft.EntityFrameworkCore;
using TimeTracker.Domain.Base;
using TimeTracker.Infrastructure.Data.SqlServer.Interfaces;

namespace TimeTracker.Infrastructure.Data.SqlServer;

public class DatabaseContext : DbContext, IDatabaseContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    #region Overrides 
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    #endregion Overrides

    #region Methods 

    public DbSet<TEntity> Set<TEntity, TId>() where TEntity : BaseModel<TId>
    {
        return base.Set<TEntity>();
    }

    #endregion Methods
}

// dotnet ef migrations add [] --startup-project ../TimeTracker.WebApi
// dotnet ef database update --startup-project ../TimeTracker.WebApi