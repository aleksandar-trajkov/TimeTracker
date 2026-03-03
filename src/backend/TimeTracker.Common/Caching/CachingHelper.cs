using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using TimeTracker.Common.Serialization;

namespace TimeTracker.Common.Caching;

public static class CachingHelper
{
    public static string GetCacheKey<T>(string prefix, T request) where T : class, ICacheable
    {
        return $"{prefix}:{request.ToString()}";
    }

    public static class CacheKeyPrefixes
    {
        public const string Users = "Users";
        public const string Categories = "Categories";
        public const string TimeEntries = "TimeEntries";
    }
}
