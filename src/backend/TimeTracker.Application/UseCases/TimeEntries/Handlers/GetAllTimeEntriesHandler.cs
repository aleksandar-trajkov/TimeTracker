using MediatR;
using TimeTracker.Application.Interfaces.Auth;
using TimeTracker.Application.Interfaces.Data;
using TimeTracker.Common.Caching;
using TimeTracker.Domain;

namespace TimeTracker.Application.UseCases.TimeEntries.Handlers;

public class GetAllTimeEntriesHandler : IRequestHandler<GetAllTimeEntriesHandler.Query, List<TimeEntry>>
{
    private readonly IUserContext _userContext;
    private readonly ITimeEntryRepository _timeEntryRepository;

    public GetAllTimeEntriesHandler(IUserContext userContext, ITimeEntryRepository timeEntryRepository)
    {
        _userContext = userContext;
        _timeEntryRepository = timeEntryRepository;
    }

    public Task<List<TimeEntry>> Handle(Query request, CancellationToken cancellationToken)
    {
        var userId = _userContext.UserId;
        return _timeEntryRepository.GetAllAsync(userId, request.From, request.To, cancellationToken);
    }
    public record Query : Cacheable, IRequest<List<TimeEntry>>
    {
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }

        public Query(DateTimeOffset from, DateTimeOffset to)
        {
            From = from;
            To = to;
            CacheKeyPrefix = CachingKeys.TimeEntries;
        }

        public override string GetCacheKey()
        {
            return $"{From:O}_{To:O}";
        }
    }
}