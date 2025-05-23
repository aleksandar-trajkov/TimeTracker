namespace TimeTracker.UnitTests.Common.Builders;

public abstract class EntityBuilder<TBuilder, TEntity> where TBuilder : EntityBuilder<TBuilder, TEntity>, new()
    where TEntity : class
{
    protected abstract TEntity Instance { get; }

    public static TEntity Build(Func<TBuilder, TEntity> configurator = null)
    {
        var builder = new TBuilder();
        return configurator?.Invoke(builder) ?? builder.Instance;
    }

    protected TBuilder With(Action<TEntity> action)
    {
        action(Instance);
        return (TBuilder)this;
    }
}