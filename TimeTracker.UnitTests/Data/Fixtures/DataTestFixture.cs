using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using TimeTracker.Domain.Base;
using TimeTracker.Infrastructure.Data.SqlServer;
using TimeTracker.Infrastructure.Data.SqlServer.Interfaces;

namespace TimeTracker.UnitTests.Data.Fixtures;

public class DataTestFixture : IDisposable
{
    private readonly DatabaseContext _context;
    private readonly IServiceProvider _serviceProvider;

    public DataTestFixture()
    {
        // Initialize the in-memory database context
        var services = new ServiceCollection();
        services.AddDbContext<DatabaseContext>(options =>
        {

            options.UseInMemoryDatabase("InMemoryDbForTesting");
            options.ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });

        _serviceProvider = services.BuildServiceProvider();
        _context = _serviceProvider.GetRequiredService<DatabaseContext>();
    }

    public DatabaseContext Context => _context;

    public void Seed<T>(IEnumerable<BaseModel<T>> seedData)
    {
        foreach (var item in seedData)
        {
            _context.Entry(item).State = EntityState.Added;
        }
        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
