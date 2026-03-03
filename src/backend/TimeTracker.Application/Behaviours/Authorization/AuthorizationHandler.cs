using LiteBus.Commands.Abstractions;
using LiteBus.Messaging.Abstractions;
using LiteBus.Queries.Abstractions;
using TimeTracker.Application.Interfaces.Auth;

namespace TimeTracker.Application.Behaviours.Authorization;

public class AuthorizationHandler<TRequest> : IAsyncMessagePreHandler<TRequest> where TRequest : class
{
    private readonly IEnumerable<IAuthorizator<TRequest>> _authorizators;
    public AuthorizationHandler(IEnumerable<IAuthorizator<TRequest>> authorizators)
    {
        _authorizators = authorizators;
    }

    public async Task PreHandleAsync(TRequest request, CancellationToken cancellationToken)
    {
        foreach (var authorizator in _authorizators)
        {
            await authorizator.AuthorizeAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}