using MediatR;
using TimeTracker.Application.Interfaces.Auth;
using TimeTracker.Application.Interfaces.Data;
using TimeTracker.Application.UseCases.Caching.Handlers;
using TimeTracker.Common.Caching;

namespace TimeTracker.Application.UseCases.TimeEntries.Handlers;

public class DeleteTimeEntryHandler : IRequestHandler<DeleteTimeEntryHandler.Command>
{
    private readonly IUserContext _userContext;
    private readonly ITimeEntryRepository _timeEntryRepository;
    private readonly IMediator _mediator;

    public DeleteTimeEntryHandler(IUserContext userContext, ITimeEntryRepository timeEntryRepository, IMediator mediator)
    {
        _userContext = userContext;
        _timeEntryRepository = timeEntryRepository;
        _mediator = mediator;
    }

    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
        var existingTimeEntry = await _timeEntryRepository.GetByIdAsync(request.Id, cancellationToken);

        // Ensure the time entry belongs to the current user
        if (existingTimeEntry.UserId != _userContext.UserId)
        {
            throw new UnauthorizedAccessException("You can only delete your own time entries.");
        }

        await _timeEntryRepository.DeleteAsync(request.Id, cancellationToken);

        await _mediator.Publish(new ClearCacheHandler.Command(CachingKeys.TimeEntries), cancellationToken);
    }

    public record Command(Guid Id) : IRequest;
}