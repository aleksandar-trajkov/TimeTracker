using LiteBus.Queries.Abstractions;
using TimeTracker.Application.Interfaces.Auth;
using TimeTracker.Application.Interfaces.Data;
using TimeTracker.Domain;

namespace TimeTracker.Application.UseCases.TimeEntries.Handlers;

public class GetTimeEntryByIdHandler : IQueryHandler<GetTimeEntryByIdHandler.Query, TimeEntry>
{
    private readonly IUserContext _userContext;
    private readonly ITimeEntryRepository _timeEntryRepository;

    public GetTimeEntryByIdHandler(IUserContext userContext, ITimeEntryRepository timeEntryRepository)
    {
        _userContext = userContext;
        _timeEntryRepository = timeEntryRepository;
    }

    public async Task<TimeEntry> HandleAsync(Query request, CancellationToken cancellationToken)
    {
        var timeEntry = await _timeEntryRepository.GetByIdAsync(request.Id, cancellationToken);

        // Ensure the time entry belongs to the current user
        if (timeEntry.UserId != _userContext.UserId)
        {
            throw new UnauthorizedAccessException("You can only access your own time entries.");
        }

        return timeEntry;
    }

    public record Query(Guid Id) : IQuery<TimeEntry>;
}