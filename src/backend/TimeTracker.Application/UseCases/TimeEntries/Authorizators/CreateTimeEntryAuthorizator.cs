using TimeTracker.Application.Interfaces.Auth;
using TimeTracker.Application.Interfaces.Data;
using TimeTracker.Application.UseCases.TimeEntries.Handlers;
using TimeTracker.Domain.Auth;
using TimeTracker.Domain.Exceptions;

namespace TimeTracker.Application.UseCases.TimeEntries.Authorizators;

public class CreateTimeEntryAuthorizator : IAuthorizator<CreateTimeEntryHandler.Command>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserContext _userContext;

    public CreateTimeEntryAuthorizator(IUserRepository userRepository, IUserContext userContext)
    {
        _userRepository = userRepository;
        _userContext = userContext;
    }

    public async Task AuthorizeAsync(CreateTimeEntryHandler.Command request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(_userContext.UserId);

        if (!user.Permissions.Any(x => x.Key == PermissionEnum.CanEditOwnTimeEntry || x.Key == PermissionEnum.CanEditAnyTimeEntry))
        {
            throw new AuthorizationException("User does not have permission to create time entries.", user.Email, "CanEditOwnTimeEntry");
        }
    }
}
