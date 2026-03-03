using System;
using System.Collections.Generic;
using System.Text;

namespace TimeTracker.Common.Caching;

public interface ICacheable
{
    string CachePrefix { get; set; }

    string GetCacheKey();
}
