namespace TimeTracker.Domain.Base;

public class BaseModel<T>
{
    public T Id { get; init; } = default!;
}
