using LiteBus.Queries.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using TimeTracker.Common.Caching;

namespace TimeTracker.Application.Behaviours.Caching;

public class CachingHandler<TRequest> : IQueryPreHandler<TRequest>, IQueryPostHandler<TRequest> where TRequest : class, IQuery
{
    public CachingHandler()
    {

    }

    public Task PreHandleAsync(TRequest message, CancellationToken cancellationToken = default)
    {
        if (message is ICacheable cacheable)
        {
            var cacheKey = cacheable.GetCacheKey();
            //var cachedResult = CacheManager.Instance.Get(cacheable.CacheKey);
            //if (cachedResult is not null)
            //{
            //    throw new CachedResultFoundException(cachedResult);
            //}
        }
        return Task.CompletedTask;
    }

    public Task PostHandleAsync(TRequest message, object? messageResult, CancellationToken cancellationToken = default)
    {
        if(message is ICacheable cacheable && messageResult is not null)
        {
            var cacheKey = cacheable.GetCacheKey();
            //CacheManager.Instance.Set(cacheable.CacheKey, messageResult, cacheable.CacheDuration);
        }
        return Task.CompletedTask;
    }
}
