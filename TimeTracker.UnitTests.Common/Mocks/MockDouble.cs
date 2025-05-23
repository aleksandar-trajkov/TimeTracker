using NSubstitute;

namespace TimeTracker.UnitTests.Common.Mocks;

public abstract class MockDouble<T> where T : class
{
    protected T Mock { get; private set; }
    protected MockDouble() => Mock = Substitute.For<T>();
}