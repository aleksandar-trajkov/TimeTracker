namespace TimeTracker.UnitTests.Common.Builders;

public abstract class EntityBuilder<TBuilder, TEntity> where TBuilder : EntityBuilder<TBuilder, TEntity>, new()
    where TEntity : class
{
    protected abstract TEntity Instance { get; }

    public TEntity Build(Func<TBuilder, TBuilder>? configurator = null)
    {
        return this.Instance;
    }

    protected TBuilder With(Action<TEntity> action)
    {
        action(Instance);
        return (TBuilder)this;
    }
}