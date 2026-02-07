using NSubstitute;

namespace TimeTracker.UnitTests.Common.Mocks;

public abstract class MockDouble<T> where T : class
{
    public T Instance { get; private set; }
    protected MockDouble() => Instance = Substitute.For<T>();
}