using System;
using System.Collections.Generic;
using System.Text;

namespace TimeTracker.Application.Interfaces.Common;

public interface ICache
{
    T? Get<T>(string prefix, string key);
    void Set<T>(string prefix, string key, T value, TimeSpan? relativeExpiration);
    void RemovePrefix(string prefix);
    void Cleanup();
}
