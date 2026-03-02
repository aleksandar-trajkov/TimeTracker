using LiteBus.Queries.Abstractions;
using TimeTracker.Application.Interfaces.Auth;
using TimeTracker.Application.Interfaces.Data;
using TimeTracker.Domain;

namespace TimeTracker.Application.UseCases.TimeEntries.Handlers;

public class GetAllTimeEntriesHandler : IQueryHandler<GetAllTimeEntriesHandler.Query, List<TimeEntry>>
{
    private readonly IUserContext _userContext;
    private readonly ITimeEntryRepository _timeEntryRepository;

    public GetAllTimeEntriesHandler(IUserContext userContext, ITimeEntryRepository timeEntryRepository)
    {
        _userContext = userContext;
        _timeEntryRepository = timeEntryRepository;
    }

    public Task<List<TimeEntry>> HandleAsync(Query request, CancellationToken cancellationToken)
    {
        var userId = _userContext.UserId;
        return _timeEntryRepository.GetAllAsync(userId, request.From, request.To, cancellationToken);
    }
    public record Query(DateTimeOffset From, DateTimeOffset To) : IQuery<List<TimeEntry>>;
}