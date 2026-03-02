using LiteBus.Commands.Abstractions;
using LiteBus.Messaging.Abstractions;
using LiteBus.Queries.Abstractions;
using TimeTracker.Application.Interfaces.Auth;

namespace TimeTracker.Application.Behaviours;

public class CommandAuthorizationBehavior<TRequest> : AuthorizationBehavior<TRequest>, ICommandPreHandler<TRequest> where TRequest : class, ICommand
{
    public CommandAuthorizationBehavior(IEnumerable<IAuthorizator<TRequest>> authorizators) : base(authorizators) { }

    public async Task PreHandleAsync(TRequest request, CancellationToken cancellationToken)
    {
        await AuthorizeAsync(request, cancellationToken).ConfigureAwait(false);
    }
}

public class QueryAuthorizationBehavior<TRequest> : AuthorizationBehavior<TRequest>, IQueryPreHandler<TRequest> where TRequest : class, IQuery
{
    public QueryAuthorizationBehavior(IEnumerable<IAuthorizator<TRequest>> authorizators) : base(authorizators) { }

    public async Task PreHandleAsync(TRequest request, CancellationToken cancellationToken)
    {
        await AuthorizeAsync(request, cancellationToken).ConfigureAwait(false);
    }
}

public class AuthorizationBehavior<TRequest> where TRequest : class
{
    private readonly IEnumerable<IAuthorizator<TRequest>> _authorizators;
    public AuthorizationBehavior(IEnumerable<IAuthorizator<TRequest>> authorizators)
    {
        _authorizators = authorizators;
    }

    public async Task AuthorizeAsync(TRequest request, CancellationToken cancellationToken)
    {
        foreach (var authorizator in _authorizators)
        {
            await authorizator.AuthorizeAsync(request, cancellationToken);
        }
    }
}