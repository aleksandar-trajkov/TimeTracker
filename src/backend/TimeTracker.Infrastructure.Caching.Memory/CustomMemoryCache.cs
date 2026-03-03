using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using System.Collections.Concurrent;
using TimeTracker.Application.Interfaces.Common;

namespace TimeTracker.Infrastructure.Caching.Memory;

public class CustomMemoryCache : ICache
{
    private readonly IMemoryCache _memoryCache;
    private readonly ConcurrentDictionary<string, CancellationTokenSource> _prefixTokens = new();

    public CustomMemoryCache(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public T? Get<T>(string prefix, string key)
    {
        if (_memoryCache.TryGetValue($"{prefix}:{key}", out T? value))
        {
            return value;
        }
        return default(T?);
    }

    /// <summary>
    /// Adds an item to the cache tied to a specific prefix.
    /// </summary>
    public void Set<T>(string prefix, string key, T value, TimeSpan? expiration = null)
    {
        var cts = GetOrAddValidToken(prefix);

        var options = new MemoryCacheEntryOptions()
            .AddExpirationToken(new CancellationChangeToken(cts.Token));

        if (expiration.HasValue)
            options.SetAbsoluteExpiration(expiration.Value);

        // We store the key with the prefix to avoid collisions across prefixes
        _memoryCache.Set($"{prefix}:{key}", value, options);
    }

    /// <summary>
    /// Instantly invalidates all entries associated with this prefix.
    /// </summary>
    public void RemovePrefix(string prefix)
    {
        if (_prefixTokens.TryRemove(prefix, out var cts))
        {
            cts.Cancel();
            cts.Dispose();
        }
    }

    /// <summary>
    /// Removes any tracking tokens that have already been cancelled. 
    /// Run this periodically (e.g., via a BackgroundService).
    /// </summary>
    public void Cleanup()
    {
        foreach (var kvp in _prefixTokens)
        {
            if (kvp.Value.IsCancellationRequested)
            {
                if (_prefixTokens.TryRemove(kvp.Key, out var cts))
                {
                    cts.Dispose();
                }
            }
        }
    }

    private CancellationTokenSource GetOrAddValidToken(string prefix)
    {
        var cts = _prefixTokens.GetOrAdd(prefix, _ => new CancellationTokenSource());

        // If the token was cancelled but hasn't been cleaned up yet, 
        // replace it so the new item isn't immediately expired.
        if (cts.IsCancellationRequested)
        {
            _prefixTokens.TryRemove(prefix, out _);
            cts.Dispose();
            cts = _prefixTokens.GetOrAdd(prefix, _ => new CancellationTokenSource());
        }

        return cts;
    }

    public void Dispose()
    {
        foreach (var cts in _prefixTokens.Values)
        {
            cts.Dispose();
        }
        _prefixTokens.Clear();
    }
}
