using TimeTracker.Application.Interfaces.Auth;
using TimeTracker.Application.Interfaces.Data;
using TimeTracker.Application.UseCases.TimeEntries.Handlers;
using TimeTracker.Domain.Auth;
using TimeTracker.Domain.Exceptions;

namespace TimeTracker.Application.UseCases.TimeEntries.Authorizators;

public class UpdateTimeEntryAuthorizator : IAuthorizator<UpdateTimeEntryHandler.Command>
{
    private readonly ITimeEntryRepository _timeEntryRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserContext _userContext;

    public UpdateTimeEntryAuthorizator(ITimeEntryRepository timeEntryRepository, IUserRepository userRepository, IUserContext userContext)
    {
        _timeEntryRepository = timeEntryRepository;
        _userRepository = userRepository;
        _userContext = userContext;
    }
    public async Task AuthorizeAsync(UpdateTimeEntryHandler.Command request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(_userContext.UserId);
        var timeEntry = await _timeEntryRepository.GetByIdAsync(request.Id, cancellationToken);

        if (timeEntry.UserId == _userContext.UserId && !user.Permissions.Any(x => x.Key == PermissionEnum.CanEditOwnTimeEntry || x.Key == PermissionEnum.CanEditAnyTimeEntry))
        {
            throw new AuthorizationException(user.Email, "User does not have permission to edit own time entries.");
        }   
        else if(!user.Permissions.Any(x => x.Key == PermissionEnum.CanEditAnyTimeEntry))
        {
            throw new AuthorizationException(user.Email, "User does not have permission to edit own time entries.");
        }
    }
}