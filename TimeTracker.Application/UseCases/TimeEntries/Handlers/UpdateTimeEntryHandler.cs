using MediatR;
using TimeTracker.Application.Interfaces.Auth;
using TimeTracker.Application.Interfaces.Data;
using TimeTracker.Domain;

namespace TimeTracker.Application.UseCases.TimeEntries.Handlers;

public class UpdateTimeEntryHandler : IRequestHandler<UpdateTimeEntryHandler.Command, bool>
{
    private readonly IUserContext _userContext;
    private readonly ITimeEntryRepository _timeEntryRepository;

    public UpdateTimeEntryHandler(IUserContext userContext, ITimeEntryRepository timeEntryRepository)
    {
        _userContext = userContext;
        _timeEntryRepository = timeEntryRepository;
    }

    public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
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

        return result > 0;
    }

    public record Command(Guid Id, DateTimeOffset From, DateTimeOffset To, string Description, Guid CategoryId) : IRequest<bool>;
}