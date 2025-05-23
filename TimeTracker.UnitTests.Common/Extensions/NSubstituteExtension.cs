using NSubstitute;
using NSubstitute.Core;

namespace TimeTracker.UnitTests.Common.Extensions;

public static class NSubstituteExtension
{
    public static ConfiguredCall ReturnsAsync<T>(this Task<T> value, T returnThis, params T[] returnThese)
    {
        return value.Returns(Task.FromResult(returnThis), returnThese.Select(Task.FromResult).ToArray());
    }

    public static ConfiguredCall ReturnsAsync<T>(this Task<T> value, Func<CallInfo, T> returnThis, params Func<CallInfo, T>[] returnThese)
    {
        var returnTheseAsyncResults = returnThese.Select<Func<CallInfo, T>, Func<CallInfo, Task<T>>>(
            x => k => Task.FromResult(x(k))).ToArray();
        return value.Returns(
            x => Task.FromResult(returnThis(x)),
            returnTheseAsyncResults);
    }
}
