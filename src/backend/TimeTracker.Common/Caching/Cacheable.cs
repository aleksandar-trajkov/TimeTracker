using System;
using System.Collections.Generic;
using System.Text;

namespace TimeTracker.Common.Caching;

public abstract record Cacheable
{
    public abstract string GetCacheKey();
    public string CacheKeyPrefix { get; protected set; } = string.Empty;

    public TimeSpan CacheDuration { get; set; } = TimeSpan.FromMinutes(5);
}
