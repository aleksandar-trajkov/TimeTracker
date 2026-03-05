using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using TimeTracker.Application.Interfaces.Common;
using TimeTracker.Common.Caching;

namespace TimeTracker.Application.Behaviours;

public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
{
    private readonly ICache _cache;

    public CachingBehavior(ICache cache)
    {
        _cache = cache;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(next);

        if (request is Cacheable cacheable)
        {
            var cacheKey = cacheable.GetCacheKey();
            var cachedResponse = _cache.Get<TResponse>(cacheable.CacheKeyPrefix, cacheKey);
            if (cachedResponse != null)
            {
                return cachedResponse;
            }

            var response = await next().ConfigureAwait(false);

            _cache.Set(cacheable.CacheKeyPrefix, cacheKey, response, cacheable.CacheDuration);
            return response;
        }

        return await next().ConfigureAwait(false);
    }
}
