namespace TimeTracker.WebApi.Contracts.Responses;

public record ResponseWrapper<T>
{
    public T Data { get; set; } = default!;
}

