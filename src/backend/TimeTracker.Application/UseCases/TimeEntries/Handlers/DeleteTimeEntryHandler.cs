using MediatR;
using TimeTracker.Application.Interfaces.Auth;
using TimeTracker.Application.Interfaces.Data;

namespace TimeTracker.Application.UseCases.TimeEntries.Handlers;

public class DeleteTimeEntryHandler : IRequestHandler<DeleteTimeEntryHandler.Command>
{
    private readonly IUserContext _userContext;
    private readonly ITimeEntryRepository _timeEntryRepository;

    public DeleteTimeEntryHandler(IUserContext userContext, ITimeEntryRepository timeEntryRepository)
    {
        _userContext = userContext;
        _timeEntryRepository = timeEntryRepository;
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
    }

    public record Command(Guid Id) : IRequest;
}