using TimeTracker.Application.Interfaces.Auth;
using TimeTracker.Application.Interfaces.Data;
using TimeTracker.Application.UseCases.TimeEntries.Handlers;
using TimeTracker.Domain.Auth;
using TimeTracker.Domain.Exceptions;

namespace TimeTracker.Application.UseCases.TimeEntries.Authorizators;

public class DeleteTimeEntryAuthorizator : IAuthorizator<DeleteTimeEntryHandler.Command>
{
    private readonly ITimeEntryRepository _timeEntryRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserContext _userContext;

    public DeleteTimeEntryAuthorizator(ITimeEntryRepository timeEntryRepository, IUserRepository userRepository, IUserContext userContext)
    {
        _timeEntryRepository = timeEntryRepository;
        _userRepository = userRepository;
        _userContext = userContext;
    }

    public async Task AuthorizeAsync(DeleteTimeEntryHandler.Command request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(_userContext.UserId);
        var timeEntry = await _timeEntryRepository.GetByIdAsync(request.Id, cancellationToken);

        if (timeEntry.UserId == _userContext.UserId)
        {
            if (!user.Permissions.Any(x => x.Key == PermissionEnum.CanEditOwnTimeEntry || x.Key == PermissionEnum.CanEditAnyTimeEntry))
            {
                throw new AuthorizationException("User does not have permission to delete own time entries.", user.Email, PermissionEnum.CanEditOwnTimeEntry.ToString());
            }
        }
        else if (!user.Permissions.Any(x => x.Key == PermissionEnum.CanEditAnyTimeEntry))
        {
            throw new AuthorizationException("User does not have permission to delete other users' time entries.", user.Email, PermissionEnum.CanEditAnyTimeEntry.ToString());
        }
    }
}
