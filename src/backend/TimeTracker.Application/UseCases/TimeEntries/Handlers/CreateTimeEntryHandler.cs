using MediatR;
using TimeTracker.Application.Interfaces.Auth;
using TimeTracker.Application.Interfaces.Data;
using TimeTracker.Application.UseCases.Caching.Handlers;
using TimeTracker.Common.Caching;
using TimeTracker.Domain;

namespace TimeTracker.Application.UseCases.TimeEntries.Handlers;

public class CreateTimeEntryHandler : IRequestHandler<CreateTimeEntryHandler.Command, TimeEntry>
{
    private readonly IUserContext _userContext;
    private readonly ITimeEntryRepository _timeEntryRepository;
    private readonly IMediator _mediator;

    public CreateTimeEntryHandler(IUserContext userContext, ITimeEntryRepository timeEntryRepository, IMediator mediator)
    {
        _userContext = userContext;
        _timeEntryRepository = timeEntryRepository;
        _mediator = mediator;
    }

    public async Task<TimeEntry> Handle(Command request, CancellationToken cancellationToken)
    {
        var timeEntry = new TimeEntry
        {
            Id = Guid.NewGuid(),
            From = request.From,
            To = request.To,
            Description = request.Description,
            CategoryId = request.CategoryId,
            UserId = _userContext.UserId
        };

        await _timeEntryRepository.InsertAsync(timeEntry, true, cancellationToken);

        await _mediator.Publish(new ClearCacheHandler.Command(CachingKeys.TimeEntries), cancellationToken);

        return timeEntry;
    }

    public record Command(DateTimeOffset From, DateTimeOffset To, string Description, Guid CategoryId) : IRequest<TimeEntry>;
}