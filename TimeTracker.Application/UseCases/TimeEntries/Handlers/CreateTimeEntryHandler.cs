using MediatR;
using TimeTracker.Application.Interfaces.Auth;
using TimeTracker.Application.Interfaces.Data;
using TimeTracker.Domain;

namespace TimeTracker.Application.UseCases.TimeEntries.Handlers;

public class CreateTimeEntryHandler : IRequestHandler<CreateTimeEntryHandler.Command, Guid>
{
    private readonly IUserContext _userContext;
    private readonly ITimeEntryRepository _timeEntryRepository;

    public CreateTimeEntryHandler(IUserContext userContext, ITimeEntryRepository timeEntryRepository)
    {
        _userContext = userContext;
        _timeEntryRepository = timeEntryRepository;
    }

    public async Task<Guid> Handle(Command request, CancellationToken cancellationToken)
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

        return timeEntry.Id;
    }

    public record Command(DateTimeOffset From, DateTimeOffset To, string Description, Guid CategoryId) : IRequest<Guid>;
}