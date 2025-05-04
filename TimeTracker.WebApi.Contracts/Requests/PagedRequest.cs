namespace TimeTracker.WebApi.Contracts.Requests;

public record PagedRequest
{
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
}
