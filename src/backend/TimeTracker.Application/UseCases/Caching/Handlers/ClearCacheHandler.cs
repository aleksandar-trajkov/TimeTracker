using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using TimeTracker.Application.Interfaces.Common;

namespace TimeTracker.Application.UseCases.Caching.Handlers;

public class ClearCacheHandler : INotificationHandler<ClearCacheHandler.Command>
{
    private readonly ICache _cache;

    public ClearCacheHandler(ICache cache)
    {
        _cache = cache;
    }
    public async Task Handle(Command notification, CancellationToken cancellationToken)
    {
        _cache.RemovePrefix(notification.CacheKeyPrefix);
        await Task.CompletedTask;  
    }

    public record Command(string CacheKeyPrefix) : INotification;
}
