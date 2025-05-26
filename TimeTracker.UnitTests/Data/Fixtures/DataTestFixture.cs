using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using Testcontainers.MsSql;
using TimeTracker.Domain.Base;
using TimeTracker.Infrastructure.Data.SqlServer;
using TimeTracker.Infrastructure.Data.SqlServer.Interfaces;

namespace TimeTracker.UnitTests.Data.Fixtures;

public class DataTestFixture : IDisposable
{
    private readonly MsSqlContainer _dbContainer = new MsSqlBuilder()
        .WithName($"test-db-{Guid.NewGuid()}")
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .Build();

    private readonly DatabaseContext _context;
    private readonly IServiceProvider _serviceProvider;

    public DataTestFixture()
    {
        // Initialize the in-memory database context
        var services = new ServiceCollection();
        services.AddDbContext<DatabaseContext>(options =>
        {

            options.UseSqlServer(_dbContainer.GetConnectionString(), x =>
            {
                x.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            });
            options.ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });

        _dbContainer.StartAsync().GetAwaiter().GetResult();

        _serviceProvider = services.BuildServiceProvider();
        _context = _serviceProvider.GetRequiredService<DatabaseContext>();

        _context.Database.EnsureCreated();
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
        _dbContainer.StopAsync().GetAwaiter().GetResult();
    }
}
