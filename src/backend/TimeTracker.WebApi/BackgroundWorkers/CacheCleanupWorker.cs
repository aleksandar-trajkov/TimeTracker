using TimeTracker.Application.Interfaces.Common;

namespace TimeTracker.WebApi.BackgroundWorkers;

public class CacheCleanupWorker : BackgroundService
{
    private readonly ICache _myCache;

    public CacheCleanupWorker(ICache myCache) => _myCache = myCache;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Run every 30 minutes to keep the dictionary small
            await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
            _myCache.Cleanup();
        }
    }
}