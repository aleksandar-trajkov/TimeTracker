using MediatR;
using TimeTracker.Application.Interfaces.Auth;
using TimeTracker.Application.Interfaces.Data;
using TimeTracker.Application.UseCases.Caching.Handlers;
using TimeTracker.Common.Caching;
using TimeTracker.Domain;

namespace TimeTracker.Application.UseCases.TimeEntries.Handlers;

public class UpdateTimeEntryHandler : IRequestHandler<UpdateTimeEntryHandler.Command, TimeEntry>
{
    private readonly IUserContext _userContext;
    private readonly ITimeEntryRepository _timeEntryRepository;
    private readonly IMediator _mediator;

    public UpdateTimeEntryHandler(IUserContext userContext, ITimeEntryRepository timeEntryRepository, IMediator mediator)
    {
        _userContext = userContext;
        _timeEntryRepository = timeEntryRepository;
        _mediator = mediator;
    }

    public async Task<TimeEntry> Handle(Command request, CancellationToken cancellationToken)
    {
        var existingTimeEntry = await _timeEntryRepository.GetByIdAsync(request.Id, cancellationToken);

        // Ensure the time entry belongs to the current user
        if (existingTimeEntry.UserId != _userContext.UserId)
        {
            throw new UnauthorizedAccessException("You can only update your own time entries.");
        }

        existingTimeEntry.From = request.From;
        existingTimeEntry.To = request.To;
        existingTimeEntry.Description = request.Description;
        existingTimeEntry.CategoryId = request.CategoryId;

        var result = await _timeEntryRepository.UpdateAsync(existingTimeEntry, true, cancellationToken);

        await _mediator.Publish(new ClearCacheHandler.Command(CachingKeys.TimeEntries), cancellationToken);

        return existingTimeEntry;
    }

    public record Command(Guid Id, DateTimeOffset From, DateTimeOffset To, string Description, Guid CategoryId) : IRequest<TimeEntry>;
}